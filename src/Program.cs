using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace IPK_proj1
{
	class Program
	{
		public static void StartListening(int port)
		{
			string data = null;

			// Data buffer for incoming data.  
			byte[] bytes = new Byte[1024];

			// Establish the local endpoint for the socket.  
			// Dns.GetHostName returns the name of the   
			// host running the application.  
			IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
			IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

			// Create a TCP/IP socket.  
			Socket listener = new Socket(ipAddress.AddressFamily,
				SocketType.Stream, ProtocolType.Tcp);

			// Bind the socket to the local endpoint and   
			// listen for incoming connections.  
			try
			{
				listener.Bind(localEndPoint);
				listener.Listen(10);

				// Start listening for connections.  
				while (true)
				{
					// Program is suspended while waiting for an incoming connection.  
					Socket handler = listener.Accept();
					data = null;




					int bytesRec = handler.Receive(bytes);
					data = Encoding.ASCII.GetString(bytes, 0, bytesRec);

					//getting first line
					string[] lines = data.Split('\n');
						//new[] { Environment.NewLine },
						//StringSplitOptions.None);

					string[] firstLine = lines[0].Split(' ');
					string method = firstLine[0];
					string requestHeader = firstLine[1];
					string[] requestBody = new string[1024];

					
					if (method == "POST")
					{
						int pos = Array.IndexOf(lines, "\r");
						Array.Copy(lines, pos + 1, requestBody, 0, lines.Length - pos - 1);
						requestBody = requestBody.Where(c => c != null).ToArray();

						if (requestBody.Length == 2)
						{
							requestBody[1] = null;
							requestBody = requestBody.Where(c => c != null).ToArray();
						}
						requestBody[requestBody.Length - 1] += "\r";
					}
					


					string address = null;
					string respondAddress = null;
					string header = "HTTP/1.1 400 Bad Request";
					string body = null;
					Regex reg;
					Regex reg2;
					bool notFound = false;



					switch (method)
					{
						case "GET":
							reg = new Regex("^/resolve\\?name=.*&type=A$");
							reg2 = new Regex("^/resolve\\?name=.*&type=PTR$");
							//if the request has the corrent form
							//type A
							if (reg.IsMatch(requestHeader))
							{
								//translation
								address = requestHeader.Substring(14, requestHeader.Length - 21);
								try
								{
									respondAddress = Dns.GetHostAddresses(address)[0].ToString();
									if (address != respondAddress)
									{
										header = "HTTP/1.1 200 OK";
										body = "\r\n\r\n" + address + ":A=" + respondAddress;
									}
								}
								catch (Exception e)
								{
									header = "HTTP/1.1 404 Not Found";
								}
							}
							//type PTR
							else if (reg2.IsMatch(requestHeader))
							{
								//translation
								address = requestHeader.Substring(14, requestHeader.Length - 23);
								try
								{
									respondAddress = Dns.GetHostEntry(address).HostName;
									if (address != respondAddress)
									{
										header = "HTTP/1.1 200 OK";
										body = "\r\n\r\n" + address + ":PTR=" + respondAddress;
									}
								}
								catch (Exception e)
								{
									header = "HTTP/1.1 404 Not Found";
								}
							}
							break;
						case "POST":
							body = "\r\n";
							reg = new Regex("^/dns-query$");
							//if the request header has the corrent form
							if (reg.IsMatch(requestHeader))
							{
								reg = new Regex("^.*:A(\r)?$");
								reg2 = new Regex("^.*:PTR(\r)?$");
								//request body has the corret form
								//type A
								foreach (string item in requestBody)
								{
									if (reg.IsMatch(item))
									{
										//translation
										if (item.Contains("\r"))
										{
											address = item.Substring(0, item.Length - 3);
										}
										else
										{
											address = item.Substring(0, item.Length - 2);
										}
										try
										{
											respondAddress = Dns.GetHostAddresses(address)[0].ToString();
											if (address != respondAddress)
											{
												header = "HTTP/1.1 200 OK";
												body = body + "\r\n" + address + ":A=" + respondAddress;
											}
										}
										catch (Exception e)
										{
											notFound = true;
										}
									}
									//request body has the correct form
									//type PTR
									else if (reg2.IsMatch(item))
									{
										//translation
										if (item.Contains("\r"))
										{
											address = item.Substring(0, item.Length - 5);
										}
										else
										{
											address = item.Substring(0, item.Length - 4);
										}
										try
										{
											respondAddress = Dns.GetHostEntry(address).HostName;
											if (address != respondAddress)
											{
												header = "HTTP/1.1 200 OK";
												body = body + "\n" + address + ":PTR=" + respondAddress;
											}
										}
										catch (Exception e)
										{
											notFound = true;
										}

									}
									else if ((item == "") || (item == "\r"))
									{
										break;
									}
									else
									{
										header = "HTTP/1.1 400 Bad Request";
										break;
									}
									if (notFound)
									{
										header = "HTTP/1.1 404 Not Found";
										break;
									}
								}
							}
							break;
						//not supported method --> error 405
						default:
							header = "HTTP/1.1 405 Method Not Allowed";
							break;
					}


					//Console.WriteLine(firstLine[0]);

					if (header != "HTTP/1.1 200 OK")
					{
						body = "\r\n";
					}

					// Echo the data back to the client.  
					byte[] msg = Encoding.ASCII.GetBytes(header + body);

					handler.Send(msg);
					handler.Shutdown(SocketShutdown.Both);
					handler.Close();
				}

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}

			Console.WriteLine("\nPress ENTER to continue...");
			Console.Read();
		}


		static void Main(string[] args)
		{
			StartListening(int.Parse(args[0]));
		}
	}
}