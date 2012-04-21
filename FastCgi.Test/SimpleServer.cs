using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastCgi.Tcp;

namespace FastCgi.Test
{
	public class SimpleServer : TcpServer
	{
		public SimpleServer(int port)
			: base(port)
		{

		}

		protected override void CreateChannel(TcpLayer tcpLayer)
		{
			//new SimpleChannelStack(tcpLayer);
			new CustomAspNetChannelStack(tcpLayer);
		}

		class SimpleChannelStack
		{
			private TcpLayer _tcpLayer;

			public SimpleChannelStack(TcpLayer tcpLayer)
			{
				_tcpLayer = tcpLayer;

				SimpleChannel channel = new SimpleChannel();
				channel.LowerLayer = tcpLayer;
				tcpLayer.UpperLayer = channel;

				channel.RequestEnded += new EventHandler(channel_RequestEnded);
			}

			void channel_RequestEnded(object sender, EventArgs e)
			{
				_tcpLayer.Close();
			}
		}

		class CustomAspNetChannelStack
		{
			private TcpLayer _tcpLayer;

			public CustomAspNetChannelStack(TcpLayer tcpLayer)
			{
				_tcpLayer = tcpLayer;

				CustomAspNetChannel channel = new CustomAspNetChannel();
				channel.LowerLayer = tcpLayer;
				tcpLayer.UpperLayer = channel;

				channel.RequestEnded += new EventHandler(channel_RequestEnded);
			}

			void channel_RequestEnded(object sender, EventArgs e)
			{
				_tcpLayer.Close();
			}
		}
	}
}
