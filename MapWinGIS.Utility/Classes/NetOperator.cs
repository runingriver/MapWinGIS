using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Microsoft.VisualBasic;

namespace MapWinGIS.Utility
{
    public class NetOperator
    {
        /// <summary>
        /// 设置的HTTP代理
        /// </summary>
        private static WebProxy m_WebProxy = null;
        /// <summary>
        /// 尝试网络连接次数
        /// </summary>
        private static int m_WebProxyNumTries = 0;

        /// <summary>
        /// 为网络连接设置代理
        /// </summary>
        /// <param name="request"></param>
        private static void SetRequestProxy(WebRequest request)
        {
            if (m_WebProxy == null) // 没有设置HTTP代理
            {
                //设置Web请求身份验证凭据
                request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }
            else // 使用设置的代理
            {
                request.Proxy = m_WebProxy;
            }
        }

        /// <summary>
        /// 根据指定URL获得一个WebResponse值
        /// </summary>
        public static WebResponse GetWebResponse(string url, WebHeaderCollection headers = null, int timeout = 20000)
        {
            WebResponse response = null;
            WebRequest request = WebRequest.Create(url);
            SetRequestProxy(request);
            try //从web获得一个回复
            {
                request.Timeout = timeout;
                if (headers != null)
                {
                    request.Headers = headers;
                }
                response = request.GetResponse();
            }
            catch (WebException we)
            {
                if (WebExceptionSetProxy(we))
                {
                    return GetWebResponse(url, headers, timeout);
                }
                else
                {
                    throw (we);
                }
            }
            return response;
        }

        /// <summary>
        /// 当网络连接异常后，从注册表获取代理信息，并再尝试三次
        /// </summary>
        private static bool WebExceptionSetProxy(WebException webException)
        {
            if (m_WebProxyNumTries < 3) //尝试连接3次
            {
                if (webException.Message.IndexOf("(407)") > -1)
                {
                    Logger.Dbg("Proxy Authentication Required");
                    m_WebProxyNumTries++;
                    string lProxyString;

                    if (m_WebProxyNumTries == 1)
                    {
                        //看我们是否已经在注册表中保存了代理信息，是，使用
                        lProxyString = Interaction.GetSetting("MapWinGIS.Utility", "Net", "ProxyServer", "");
                        if (lProxyString.Length > 0 && ProxyFromString(lProxyString) && m_WebProxy != null)
                        {
                            return true;
                        }
                    }

                    lProxyString = Interaction.InputBox("请输入代理信息", "代理设置", "servername:80:username:password", -1, -1);
                    if (ProxyFromString(lProxyString))
                    {
                        Interaction.SaveSetting("MapWinGIS.Utility", "Net", "ProxyServer", lProxyString);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 将一个字符串转换成WebProxy
        /// 80和8080是常用端口 
        /// </summary>
        /// <param name="proxyString">代理信息的格式："servername:port"\"servername:port:username:password"</param>
        /// <returns>true- 则将代理信息写入m_WebProxy</returns>
        private static bool ProxyFromString(string proxyString)
        {
            string[] lProxySettings = proxyString.Split(':');
            int port;
            if (lProxySettings.Length > 1 && int.TryParse(lProxySettings[1], out port))
            {
                m_WebProxy = new WebProxy(lProxySettings[0], port);
                if (lProxySettings.Length > 3)
                {
                    m_WebProxy.Credentials = new NetworkCredential(lProxySettings[2], lProxySettings[3]);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 执行URL并以字符串的方式返回网页回复结果
        /// </summary>
        /// <param name="fullUrl">URL</param>
        /// <param name="postdata">发送的数据</param>
        /// <param name="allowAutoRedirect">指示请求是否应跟随重定向响应</param>
        /// <param name="timeout">请求超时值</param>
        public static string ExecuteUrl(string fullUrl, string postdata, bool allowAutoRedirect = true, int timeout = System.Threading.Timeout.Infinite)
        {
            System.Net.HttpWebRequest webRequest;
            System.Net.HttpWebResponse webResponse = null;
            try
            {
                //用指定的URL创建一个HttpWebRequest.
                webRequest = (System.Net.HttpWebRequest)(System.Net.WebRequest.Create(fullUrl));
                SetRequestProxy(webRequest);
                webRequest.AllowAutoRedirect = allowAutoRedirect;
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = postdata.Length;
                webRequest.Timeout = timeout;

                Stream requestStream = webRequest.GetRequestStream();
                byte[] postBytes = System.Text.Encoding.ASCII.GetBytes(postdata);
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                //发送请求，等待回复
                try
                {
                    webResponse = (System.Net.HttpWebResponse)(webRequest.GetResponse());
                    if (webResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //读取回复内容
                        System.IO.Stream responseStream = webResponse.GetResponseStream();
                        System.Text.Encoding responseEncoding = System.Text.Encoding.UTF8;

                        System.IO.StreamReader responseReader = new System.IO.StreamReader(responseStream, responseEncoding);
                        string responseContent = responseReader.ReadToEnd();
                        return responseContent;
                    }
                    else if (((webResponse.StatusCode) == System.Net.HttpStatusCode.Redirect) || ((webResponse.StatusCode) == System.Net.HttpStatusCode.MovedPermanently))
                    {
                        throw new System.Exception(string.Format("无法读取回复内容. URL被移除. 响应状态={0}.", webResponse.StatusCode));
                    }
                    else if ((webResponse.StatusCode) == System.Net.HttpStatusCode.NotFound)
                    {
                        throw new Exception(string.Format("无法读取回复内容.URL没找到. 响应状态={0}.", webResponse.StatusCode));
                    }
                    else
                    {
                        throw new Exception(string.Format("无法读取回复内容.响应状态={0}.", webResponse.StatusCode));
                    }
                }
                catch (System.Net.WebException we)
                {
                    if (WebExceptionSetProxy(we)) //设置完网页代理后，重试一次
                    {
                        return ExecuteUrl(fullUrl, postdata, allowAutoRedirect, timeout);
                    }
                    else
                    {
                        throw (we);
                    }
                }
                finally
                {
                    if (webResponse != null)
                    {
                        webResponse.Close();
                    }
                }
            }
            catch (System.Exception e)
            {
                throw new Exception("无法执行 URL \'" + fullUrl + "\'", e);
            }
        }

        /// <summary>
        /// 从指定URL下载一系列字符串
        /// </summary>
        public static string DownloadFile(string URL)
        {
            StringBuilder downloadedString = new StringBuilder();
            try
            {
                WebResponse webResponse = GetWebResponse(URL);
                if (webResponse != null)
                {
                    string lContentLength = webResponse.Headers["Content-Length"];
                    if (lContentLength != "0") //0代表没有内容，但空代表有内容
                    {
                        Stream input = webResponse.GetResponseStream();
                        if (input != null)
                        {
                            int count = 128 * 1024; 
                            byte[] buffer = new byte[count];
                            do
                            {
                                count = input.Read(buffer, 0, count);
                                if (count == 0)
                                {
                                    break; //下载完成
                                }
                                downloadedString.Append(System.Text.Encoding.UTF8.GetString(buffer, 0, count));
                            } while (true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                downloadedString.Append(ex.ToString());
            }
            return downloadedString.ToString();
        }

        /// <summary>
        /// 根据给点的URL检测网络连接是否可用
        /// </summary>
        /// <param name="checkAgainstURL">检测网络连接的URL</param>
        /// <param name="timeoutMilliseconds">设置连接超时的时间</param>
        public static bool CheckInternetConnection(string checkAgainstURL, int timeoutMilliseconds = 2000)
        {
            WebResponse objResp = null;
            try
            {
                objResp = GetWebResponse(url:checkAgainstURL, timeout: timeoutMilliseconds);
                objResp.Close();
                return true;
            }
            catch (System.Net.WebException ex)
            {
                Logger.Dbg("网络连接异常: " + ex.ToString());
                return false;
            }
            catch (Exception ex)
            {
                Logger.Dbg("异常: " + ex.ToString());
                return false;
            }
            finally
            {
                if (objResp != null)
                {
                    objResp.Close();
                }
            }
        }

    }
}
