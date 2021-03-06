﻿using System;
using System.Security.Cryptography;
using System.Threading;
using System.Web;

namespace Escc.Web
{
    /// <summary>
    /// Configure common HTTP response statuses in a way which complies with HTTP 1.1 in RFC2116
    /// </summary>
    public class HttpStatus : IHttpStatus
    {
        /// <summary>
        /// Sets the current response status to '301 Moved Permanently' and redirects to the specified URL using HttpContext.Current
        /// </summary>
        /// <param name="replacedByUrl">The replacement URL.</param>
        /// <remarks>See <seealso cref="MovedPermanently( Uri, string,Uri, HttpResponse)"/> for more details.</remarks>
        public void MovedPermanently(string replacedByUrl)
        {
            MovedPermanently(new Uri(replacedByUrl, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Sets the current response status to '301 Moved Permanently' and redirects to the specified URL using HttpContext.Current
        /// </summary>
        /// <param name="replacedByUrl">The replacement URL.</param>
        /// <remarks>See <seealso cref="MovedPermanently( Uri, string,Uri, HttpResponse)"/> for more details.</remarks>
        public void MovedPermanently(Uri replacedByUrl)
        {
            MovedPermanently(HttpContext.Current.Request.Url, HttpContext.Current.Request.HttpMethod, replacedByUrl, HttpContext.Current.Response);
        }

        /// <summary>
        /// Sets the supplied response status to '301 Moved Permanently' and redirects to the specified URL
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="requestMethod">The request method.</param>
        /// <param name="replacedByUrl">The replacement URL.</param>
        /// <param name="response">The response.</param>
        /// <exception cref="System.ArgumentNullException">
        /// replacedByUrl
        /// or
        /// request
        /// or
        /// response
        /// </exception>
        /// <exception cref="System.ArgumentException">The request and destination URIs are the same;replacedByUrl</exception>
        /// <remarks>
        /// <para>Implements RFC2616:</para>
        /// <para>
        ///   <c>The requested resource has been assigned a new permanent URI and any future references to this resource
        /// SHOULD use one of the returned URIs. Clients with link editing capabilities ought to automatically re-link references
        /// to the Request-URI to one or more of the new references returned by the server, where possible. This response is
        /// cacheable unless indicated otherwise.</c>
        /// </para>
        /// <para>
        ///   <c>The new permanent URI SHOULD be given by the Location field in the response. Unless the request method was HEAD,
        /// the entity of the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s).</c>
        /// </para>
        /// </remarks>
        public void MovedPermanently(Uri requestUrl, string requestMethod, Uri replacedByUrl, HttpResponse response)
        {
            if (replacedByUrl == null) throw new ArgumentNullException("replacedByUrl");
            if (requestUrl == null) throw new ArgumentNullException("request");
            if (response == null) throw new ArgumentNullException("response");

            // RFC 2616 says the destination URL for the Location header must be absolute
            var absoluteDestination = replacedByUrl.IsAbsoluteUri ? replacedByUrl : new Uri(HttpContext.Current.Request.Url, replacedByUrl);

            // RFC 2616 says it must be a different URI (otherwise you'll get a redirect loop)
            if (absoluteDestination.ToString() == requestUrl.ToString()) throw new ArgumentException("The request and destination URIs are the same", "replacedByUrl");

            try
            {
                response.Status = "301 Moved Permanently";
                response.StatusCode = 301;
                response.AddHeader("Location", absoluteDestination.ToString());

                if (requestMethod != "HEAD")
                {
                    // Use a minimal HTML5 document for the hypertext response, since few people will ever see it.
                    response.Write("<!DOCTYPE html><title>This page has moved</title><h1>This page has moved</h1><p>Please see <a href=\"" + absoluteDestination + "\">" + absoluteDestination + "</a> instead.</p>");
                }

                response.End();
            }
            catch (ThreadAbortException)
            {
                // Just catch the expected exception. Don't call Thread.ResetAbort() because, 
                // in an Umbraco context, it causes problems with updating the cache.
            }
        }

        /// <summary>
        /// Sets the supplied response status to '301 Moved Permanently' and redirects to the specified URL
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="requestMethod">The request method.</param>
        /// <param name="replacedByUrl">The replacement URL.</param>
        /// <param name="response">The response.</param>
        /// <exception cref="System.ArgumentNullException">
        /// replacedByUrl
        /// or
        /// request
        /// or
        /// response
        /// </exception>
        /// <exception cref="System.ArgumentException">The request and destination URIs are the same;replacedByUrl</exception>
        /// <remarks>
        /// <para>Implements RFC2616:</para>
        /// <para>
        ///   <c>The requested resource has been assigned a new permanent URI and any future references to this resource
        /// SHOULD use one of the returned URIs. Clients with link editing capabilities ought to automatically re-link references
        /// to the Request-URI to one or more of the new references returned by the server, where possible. This response is
        /// cacheable unless indicated otherwise.</c>
        /// </para>
        /// <para>
        ///   <c>The new permanent URI SHOULD be given by the Location field in the response. Unless the request method was HEAD,
        /// the entity of the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s).</c>
        /// </para>
        /// </remarks>
        public void MovedPermanently(Uri requestUrl, string requestMethod, Uri replacedByUrl, HttpResponseBase response)
        {
            if (replacedByUrl == null) throw new ArgumentNullException("replacedByUrl");
            if (requestUrl == null) throw new ArgumentNullException("request");
            if (response == null) throw new ArgumentNullException("response");

            // RFC 2616 says the destination URL for the Location header must be absolute
            var absoluteDestination = replacedByUrl.IsAbsoluteUri ? replacedByUrl : new Uri(HttpContext.Current.Request.Url, replacedByUrl);

            // RFC 2616 says it must be a different URI (otherwise you'll get a redirect loop)
            if (absoluteDestination.ToString() == requestUrl.ToString()) throw new ArgumentException("The request and destination URIs are the same", "replacedByUrl");

            try
            {
                response.Status = "301 Moved Permanently";
                response.StatusCode = 301;
                response.AddHeader("Location", absoluteDestination.ToString());

                if (requestMethod != "HEAD")
                {
                    // Use a minimal HTML5 document for the hypertext response, since few people will ever see it.
                    response.Write("<!DOCTYPE html><title>This page has moved</title><h1>This page has moved</h1><p>Please see <a href=\"" + absoluteDestination + "\">" + absoluteDestination + "</a> instead.</p>");
                }

                response.End();
            }
            catch (ThreadAbortException)
            {
                // Just catch the expected exception. Don't call Thread.ResetAbort() because, 
                // in an Umbraco context, it causes problems with updating the cache.
            }
        }

        /// <summary>
        /// Sets the current response status to '303 See Other' and redirects to the specified URL using HttpContext.Current
        /// </summary>
        /// <param name="destinationUrl">The destination URL.</param>
        /// <remarks>See <seealso cref="SeeOther( Uri, string,Uri, HttpResponse)"/> for more details.</remarks>
        public void SeeOther(string destinationUrl)
        {
            SeeOther(new Uri(destinationUrl, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Sets the current response status to '303 See Other' and redirects to the specified URL using HttpContext.Current
        /// </summary>
        /// <param name="destinationUrl">The destination URL.</param>
        /// <remarks>See <seealso cref="SeeOther( Uri, string,Uri, HttpResponse)"/> for more details.</remarks>
        public void SeeOther(Uri destinationUrl)
        {
            SeeOther(HttpContext.Current.Request.Url, HttpContext.Current.Request.HttpMethod, destinationUrl, HttpContext.Current.Response);
        }

        /// <summary>
        /// Sets the supplied response status to '303 See Other' and redirects to the specified URL
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="requestMethod">The request method.</param>
        /// <param name="destinationUrl">The destination URL.</param>
        /// <param name="response">The response.</param>
        /// <exception cref="System.ArgumentNullException">
        /// destinationUrl
        /// or
        /// request
        /// or
        /// response
        /// </exception>
        /// <exception cref="System.ArgumentException">The request and destination URIs are the same;destinationUrl</exception>
        /// <remarks>
        /// <para>Implements RFC2616:</para>
        /// <para>
        ///   <c>The response to the request can be found under a different URI and SHOULD be retrieved using a GET method on that resource.
        /// This method exists primarily to allow the output of a POST-activated script to redirect the user agent to a selected resource.
        /// The new URI is not a substitute reference for the originally requested resource. The 303 response MUST NOT be cached, but the
        /// response to the second (redirected) request might be cacheable.</c>
        /// </para>
        /// <para>
        ///   <c>The different URI SHOULD be given by the Location field in the response. Unless the request method was HEAD, the entity of
        /// the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s).</c>
        /// </para>
        /// </remarks>
        public void SeeOther(Uri requestUrl, string requestMethod, Uri destinationUrl, HttpResponse response)
        {
            if (destinationUrl == null) throw new ArgumentNullException("destinationUrl");
            if (requestUrl == null) throw new ArgumentNullException("requestUrl");
            if (response == null) throw new ArgumentNullException("response");

            // RFC 2616 says the destination URL for the Location header must be absolute
            var absoluteDestination = destinationUrl.IsAbsoluteUri ? destinationUrl : new Uri(HttpContext.Current.Request.Url, destinationUrl);

            // RFC 2616 says it must be a different URI (otherwise you'll get a redirect loop)
            if (absoluteDestination.ToString() == requestUrl.ToString())
            {
                throw new ArgumentException("The request and destination URIs are the same", "destinationUrl");
            }

            try
            {
                response.Status = "303 See Other";
                response.StatusCode = 303;
                response.AddHeader("Location", absoluteDestination.ToString());

                if (requestMethod != "HEAD")
                {
                    // Use a minimal HTML5 document for the hypertext response, since few people will ever see it.
                    response.Write("<!DOCTYPE html><title>See another page</title><h1>See another page</h1><p>Please see <a href=\"" + absoluteDestination + "\">" + absoluteDestination + "</a> instead.</p>");
                }

                response.End();
            }
            catch (ThreadAbortException)
            {
                // Just catch the expected exception. Don't call Thread.ResetAbort() because, 
                // in an Umbraco context, it causes problems with updating the cache.
            }
        }

        /// <summary>
        /// Sets the supplied response status to '303 See Other' and redirects to the specified URL
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="requestMethod">The request method.</param>
        /// <param name="destinationUrl">The destination URL.</param>
        /// <param name="response">The response.</param>
        /// <exception cref="System.ArgumentNullException">
        /// destinationUrl
        /// or
        /// request
        /// or
        /// response
        /// </exception>
        /// <exception cref="System.ArgumentException">The request and destination URIs are the same;destinationUrl</exception>
        /// <remarks>
        /// <para>Implements RFC2616:</para>
        /// <para>
        ///   <c>The response to the request can be found under a different URI and SHOULD be retrieved using a GET method on that resource.
        /// This method exists primarily to allow the output of a POST-activated script to redirect the user agent to a selected resource.
        /// The new URI is not a substitute reference for the originally requested resource. The 303 response MUST NOT be cached, but the
        /// response to the second (redirected) request might be cacheable.</c>
        /// </para>
        /// <para>
        ///   <c>The different URI SHOULD be given by the Location field in the response. Unless the request method was HEAD, the entity of
        /// the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s).</c>
        /// </para>
        /// </remarks>
        public void SeeOther(Uri requestUrl, string requestMethod, Uri destinationUrl, HttpResponseBase response)
        {
            if (destinationUrl == null) throw new ArgumentNullException("destinationUrl");
            if (requestUrl == null) throw new ArgumentNullException("requestUrl");
            if (response == null) throw new ArgumentNullException("response");

            // RFC 2616 says the destination URL for the Location header must be absolute
            var absoluteDestination = destinationUrl.IsAbsoluteUri ? destinationUrl : new Uri(HttpContext.Current.Request.Url, destinationUrl);

            // RFC 2616 says it must be a different URI (otherwise you'll get a redirect loop)
            if (absoluteDestination.ToString() == requestUrl.ToString())
            {
                throw new ArgumentException("The request and destination URIs are the same", "destinationUrl");
            }

            try
            {
                response.Status = "303 See Other";
                response.StatusCode = 303;
                response.AddHeader("Location", absoluteDestination.ToString());

                if (requestMethod != "HEAD")
                {
                    // Use a minimal HTML5 document for the hypertext response, since few people will ever see it.
                    response.Write("<!DOCTYPE html><title>See another page</title><h1>See another page</h1><p>Please see <a href=\"" + absoluteDestination + "\">" + absoluteDestination + "</a> instead.</p>");
                }

                response.End();
            }
            catch (ThreadAbortException)
            {
                // Just catch the expected exception. Don't call Thread.ResetAbort() because, 
                // in an Umbraco context, it causes problems with updating the cache.
            }
        }

        /// <summary>
        /// Sets the current response status to '410 Gone' meaning the requested resource has been removed permanently and no forwarding address is known. If the resource may come back, use 404 Not Found. Uses HttpContext.Current.
        /// </summary>
        /// <remarks>See <seealso cref="Gone(HttpResponse)"/> for more details.</remarks>
        public void Gone()
        {
            Gone(HttpContext.Current.Response);
        }

        /// <summary>
        /// Sets the supplied response status to '410 Gone' meaning the requested resource has been removed permanently and no forwarding address is known. If the resource may come back, use 404 Not Found.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <remarks>
        /// 	<para>Implements RFC2616:</para>
        /// 	<para><c>The requested resource is no longer available at the server and no forwarding address is known. This condition is expected to be considered permanent. 
        /// 	Clients with link editing capabilities SHOULD delete references to the Request-URI after user approval. If the server does not know, or has no facility to determine, 
        /// 	whether or not the condition is permanent, the status code 404 (Not Found) SHOULD be used instead. This response is cacheable unless indicated otherwise.</c></para>
        /// </remarks>
        public void Gone(HttpResponse response)
        {
            if (response == null) throw new ArgumentNullException("response");

            response.Status = "410 Gone";
            response.StatusCode = 410;
        }

        /// <summary>
        /// Sets the supplied response status to '410 Gone' meaning the requested resource has been removed permanently and no forwarding address is known. If the resource may come back, use 404 Not Found.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <remarks>
        /// 	<para>Implements RFC2616:</para>
        /// 	<para><c>The requested resource is no longer available at the server and no forwarding address is known. This condition is expected to be considered permanent. 
        /// 	Clients with link editing capabilities SHOULD delete references to the Request-URI after user approval. If the server does not know, or has no facility to determine, 
        /// 	whether or not the condition is permanent, the status code 404 (Not Found) SHOULD be used instead. This response is cacheable unless indicated otherwise.</c></para>
        /// </remarks>
        public void Gone(HttpResponseBase response)
        {
            if (response == null) throw new ArgumentNullException("response");

            response.Status = "410 Gone";
            response.StatusCode = 410;
        }

        /// <summary>
        /// Sets the current response status to '400 Bad Request' due to malformed syntax. The client SHOULD NOT repeat the request without modifications. Uses HttpContext.Current.
        /// </summary>
        /// <remarks>See <seealso cref="BadRequest(HttpResponse)"/> for more details.</remarks>
        public void BadRequest()
        {
            BadRequest(HttpContext.Current.Response);
        }

        /// <summary>
        /// Sets the supplied response status to '400 Bad Request' due to malformed syntax. The client SHOULD NOT repeat the request without modifications.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <remarks>
        /// 	<para>Implements RFC2616:</para>
        /// 	<para><c>The request could not be understood by the server due to malformed syntax. The client SHOULD NOT repeat the request without modifications.</c></para>
        /// </remarks>
        public void BadRequest(HttpResponse response)
        {
            if (response == null) throw new ArgumentNullException("response");

            response.Status = "400 Bad Request";
            response.StatusCode = 400;
        }

        /// <summary>
        /// Sets the supplied response status to '400 Bad Request' due to malformed syntax. The client SHOULD NOT repeat the request without modifications.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <remarks>
        /// 	<para>Implements RFC2616:</para>
        /// 	<para><c>The request could not be understood by the server due to malformed syntax. The client SHOULD NOT repeat the request without modifications.</c></para>
        /// </remarks>
        public void BadRequest(HttpResponseBase response)
        {
            if (response == null) throw new ArgumentNullException("response");

            response.Status = "400 Bad Request";
            response.StatusCode = 400;
        }

        /// <summary>
        /// Sets the current response status to '404 Not Found' when the page is not found, or hidden for some reason. Uses HttpContext.Current.
        /// </summary>
        /// <remarks>See <seealso cref="NotFound(HttpResponse)"/> for more details.</remarks>
        public void NotFound()
        {
            NotFound(HttpContext.Current.Response);
        }

        /// <summary>
        /// Sets the supplied response status to '404 Not Found' when the page is not found, or hidden for some reason.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <remarks>
        /// 	<para>Implements RFC2616:</para>
        /// 	<para><c>The server has not found anything matching the Request-URI. No indication is given of whether the condition is temporary or permanent.
        /// 	The 410 (Gone) status code SHOULD be used if the server knows, through some internally configurable mechanism, that an old resource is permanently 
        /// 	unavailable and has no forwarding address. This status code is commonly used when the server does not wish to reveal exactly why the request has 
        /// 	been refused, or when no other response is applicable.</c></para>
        /// </remarks>
        public void NotFound(HttpResponse response)
        {
            if (response == null) throw new ArgumentNullException("response");

            response.Status = "404 Not Found";
            response.StatusCode = 404;
        }

        /// <summary>
        /// Sets the supplied response status to '404 Not Found' when the page is not found, or hidden for some reason.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <remarks>
        /// 	<para>Implements RFC2616:</para>
        /// 	<para><c>The server has not found anything matching the Request-URI. No indication is given of whether the condition is temporary or permanent.
        /// 	The 410 (Gone) status code SHOULD be used if the server knows, through some internally configurable mechanism, that an old resource is permanently 
        /// 	unavailable and has no forwarding address. This status code is commonly used when the server does not wish to reveal exactly why the request has 
        /// 	been refused, or when no other response is applicable.</c></para>
        /// </remarks>
        public void NotFound(HttpResponseBase response)
        {
            if (response == null) throw new ArgumentNullException("response");

            response.Status = "404 Not Found";
            response.StatusCode = 404;
        }

        /// <summary>
        /// Sets the current response status to '500 Internal Server Error' indicating an unexpected error. Uses HttpContext.Current.
        /// </summary>
        /// <remarks>See <seealso cref="BadRequest(HttpResponse)"/> for more details.</remarks>
        public void InternalServerError()
        {
            InternalServerError(HttpContext.Current.Response);
        }

        /// <summary>
        /// Sets the supplied response status to '500 Internal Server Error' indicating an unexpected error.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <remarks>
        /// 	<para>Implements RFC2616:</para>
        /// 	<para><c>The server encountered an unexpected condition which prevented it from fulfilling the request.</c></para>
        /// </remarks>
        public void InternalServerError(HttpResponse response)
        {
            if (response == null) throw new ArgumentNullException("response");

            response.Status = "500 Internal Server Error";
            response.StatusCode = 500;

            RandomDelay();
        }

        /// <summary>
        /// Sets the supplied response status to '500 Internal Server Error' indicating an unexpected error.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <remarks>
        /// 	<para>Implements RFC2616:</para>
        /// 	<para><c>The server encountered an unexpected condition which prevented it from fulfilling the request.</c></para>
        /// </remarks>
        public void InternalServerError(HttpResponseBase response)
        {
            if (response == null) throw new ArgumentNullException("response");

            response.Status = "500 Internal Server Error";
            response.StatusCode = 500;

            RandomDelay();
        }

        /// <summary>
        /// Introduce a random delay, so defend against anyone trying to detect specific errors based on the time taken.
        /// <remarks>
        /// Code from <a href="http://weblogs.asp.net/scottgu/archive/2010/09/18/important-asp-net-security-vulnerability.aspx">http://weblogs.asp.net/scottgu/archive/2010/09/18/important-asp-net-security-vulnerability.aspx</a>
        /// </remarks>
        /// </summary>
        private static void RandomDelay()
        {
            byte[] delay = new byte[1];
            RandomNumberGenerator prng = new RNGCryptoServiceProvider();

            prng.GetBytes(delay);
            Thread.Sleep(delay[0]);

            IDisposable disposable = prng;
            disposable.Dispose();
        }


        /// <summary>
        /// Sets the supplied response status to '502 Bad Gateway' meaning it's not our fault; a service called by the page failed.
        /// </summary>
        /// <remarks>
        ///     <para>Implements RFC2616:</para>
        ///     <para><c>The server, while acting as a gateway or proxy, received an invalid response from the upstream server it accessed in attempting to fulfill the request.</c></para>
        /// </remarks>
        /// <param name="response">The response.</param>
        public void BadGateway(HttpResponse response)
        {
            if (response == null) throw new ArgumentNullException("response");

            response.Status = "502 Bad Gateway";
            response.StatusCode = 502;

            RandomDelay();
        }

        /// <summary>
        /// Sets the supplied response status to '502 Bad Gateway' meaning it's not our fault; a service called by the page failed.
        /// </summary>
        /// <remarks>
        ///     <para>Implements RFC2616:</para>
        ///     <para><c>The server, while acting as a gateway or proxy, received an invalid response from the upstream server it accessed in attempting to fulfill the request.</c></para>
        /// </remarks>
        /// <param name="response">The response.</param>
        public void BadGateway(HttpResponseBase response)
        {
            if (response == null) throw new ArgumentNullException("response");

            response.Status = "502 Bad Gateway";
            response.StatusCode = 502;

            RandomDelay();
        }

        /// <summary>
        /// Sets the supplied response status to '502 Bad Gateway' meaning it's not our fault; a service called by the page failed. Uses HttpContext.Current.
        /// </summary>
        /// <remarks>See <seealso cref="BadGateway(HttpResponse)"/> for more details.</remarks>
        public void BadGateway()
        {
            BadGateway(HttpContext.Current.Response);
        }
    }
}
