#region License
//*****************************************************************************/
// Copyright (c) 2012 Luigi Grilli
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//*****************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using FastCgi.ImmutableArray;
using System.Diagnostics;
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;

namespace FastCgi.Protocol
{
	/// <summary>
	/// FastiCGI channel
	/// </summary>
	public abstract class FastCgiChannel : IUpperLayer
	{
		/// <summary>
		/// Event generated when a request is ended
		/// </summary>
		public event EventHandler RequestEnded;

		/// <summary>
		/// Receive buffer
		/// </summary>
		private ByteArray _recvBuffer = ByteArray.Empty;

		/// <summary>
		/// Lower layer to send messages to the FastCGI client
		/// </summary>
		public ILowerLayer LowerLayer { get; set; }

		/// <summary>
		/// Channel properties as required by protocol specifications
		/// </summary>
		public ChannelProperties Properties { get; protected set; }

		/// <summary>
		/// Receives data from the lower layer
		/// </summary>
		/// <param name="data">Data recived</param>
		public virtual void Receive(ByteArray data)
		{
            _recvBuffer = _recvBuffer.Concat(data, true);

			while (_recvBuffer.Count >= MessageHeader.HeaderSize)
			{
				MessageHeader header = new MessageHeader(_recvBuffer);
				if (_recvBuffer.Count < header.MessageSize)
					break;

				ByteArray messageData = _recvBuffer.SubArray(0, header.MessageSize);
				_recvBuffer = _recvBuffer.SubArray(header.MessageSize, true);

				this.ReceiveMessage(new Message(messageData));
			}
		}

		/// <summary>
		/// Process a received FastCGI message
		/// </summary>
		/// <param name="message"></param>
		protected virtual void ReceiveMessage(Message message)
		{
			ushort requestId = message.Header.RequestId;

			switch (message.Header.MessageType)
			{
				case MessageType.BeginRequest:
					this.BeginRequest(message);
					break;

				case MessageType.AbortRequest:
					this.GetRequest(requestId).Abort();
					break;

				case MessageType.Params:
					this.GetRequest(requestId).ParametersStream.Append(message.Body);
					break;

				case MessageType.StandardInput:
					if (message.Header.ContentLength > 0)
						this.GetRequest(requestId).InputStream.Append(message.Body);
					else
						this.GetRequest(requestId).Execute();
					break;

				case MessageType.Data:
					this.GetRequest(requestId).DataStream.Append(message.Body);
					break;

				case MessageType.GetValues:
					this.SendGetValuesResult(message.Body);
					break;

				default:
					break;
			}
		}

		/// <summary>
		/// Send a reply to a GetValues request
		/// </summary>
		/// <param name="data">GetValues message body</param>
		private void SendGetValuesResult(ByteArray data)
		{
			NameValuePairCollection collection;

			//reads the collection
			using (InputStream stream = new InputStream(data))
			{
				collection = new NameValuePairCollection(stream);
			}
			
			//sets the value
			this.Properties.GetValues(collection);

			//sends the collection
			using(MemoryStream stream = new MemoryStream())
			{
				using(BinaryWriter writer = new BinaryWriter(stream))
				{
					foreach(NameValuePair item in collection)
						item.Encode(writer);

					ByteArray body = new ByteArray(stream.ToArray());
					this.SendMessage(new Message(MessageType.GetValuesResult, 0, body));
				}
			}
		}

		/// <summary>
		/// Creates and initalize a <typeparamref name="Request"/> 
		/// </summary>
		/// <param name="message">BeginRequest message</param>
		protected virtual void BeginRequest(Message message)
		{
			if (message.Header.RecordType != RecordType.Application)
				return;

			if (message.Body.Count < Consts.ChunkSize)
				return;

			Request request = this.CreateRequest(message.Header.RequestId, new BeginRequestMessageBody(message.Body));
			request.Ended += new EventHandler(OnRequestEnded);
			request.OutputFlushing += new EventHandler<FlushEventArgs>(OnRequestOutputFlushing);
			request.ErrorFlushing += new EventHandler<FlushEventArgs>(OnRequestErrorFlushing);

			this.AddRequest(request);
		}

		private void OnRequestEnded(object sender, EventArgs e)
		{
			this.EndRequest((Request)sender);

			if (this.RequestEnded != null)
				this.RequestEnded(sender, EventArgs.Empty);
		}

		private void OnRequestOutputFlushing(object sender, FlushEventArgs e)
		{
			this.SendRequestOutput((Request)sender, MessageType.StandardOutput, e.Data);
		}

		private void OnRequestErrorFlushing(object sender, FlushEventArgs e)
		{
			this.SendRequestOutput((Request)sender, MessageType.StandardError, e.Data);
		}

		/// <summary>
		/// Ends a request
		/// </summary>
		/// <param name="request"><typeparamref name="Request"/> to end</param>
		protected virtual void EndRequest(Request request)
		{
			EndRequestMessageBody body = new EndRequestMessageBody(request.ExitCode, ProtocolStatus.RequestComplete);

			this.SendMessage(new Message(MessageType.StandardError, request.Id, ByteArray.Empty));
			this.SendMessage(new Message(MessageType.StandardOutput, request.Id, ByteArray.Empty));
			this.SendMessage(new Message(MessageType.EndRequest, request.Id, body.ToArray()));

			this.RemoveRequest(request);
		}

		/// <summary>
		/// Send a request output data to the output or error stream
		/// </summary>
		/// <param name="request">Request</param>
		/// <param name="streamType"></param>
		/// <param name="data"></param>
		protected virtual void SendRequestOutput(Request request, MessageType streamType, ByteArray data)
		{
			if (streamType != MessageType.StandardError &&
				streamType != MessageType.StandardOutput)
					throw new ArgumentException("streamType");

			while (data.Count > 0)
			{
				ushort length = (ushort)Math.Min(data.Count, Consts.MaxMessageBodySize);
				this.SendMessage(new Message(streamType, request.Id, data.SubArray(0, length)));
				data = data.SubArray(length, true);
			}
		}

		/// <summary>
		/// Sends a message to the FastCGI client
		/// </summary>
		/// <param name="message"><typeparamref name="Message"/> to send</param>
		public virtual void SendMessage(Message message)
		{
			var array = message.ToByteArray();
			Debug.WriteLine(array);

			this.LowerLayer.Send(array);
		}

		#region Abstract methods

		protected abstract Request CreateRequest(ushort requestId, BeginRequestMessageBody body);

		protected abstract void AddRequest(Request request);

		protected abstract void RemoveRequest(Request request);

		protected abstract Request GetRequest(ushort requestId);

		#endregion
	}
}
