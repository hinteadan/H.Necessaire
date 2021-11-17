using Bridge.Html5;
using Bridge.jQuery2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class HttpClient : ImADependency
    {
        public void ReferDependencies(ImADependencyProvider dependencyProvider) { }

        private readonly Dictionary<string, string> customHeaders = new Dictionary<string, string>();

        public HttpClient SetAuth(string type, string token)
        {
            customHeaders["Authorization"] = $"{type} {token}";
            return this;
        }

        public HttpClient SetConsumer(Guid consumerID)
        {
            customHeaders["X-H.Necessaire.ConsumerID"] = $"{consumerID}";
            return this;
        }

        public HttpClient ZapAuth()
        {
            if (!customHeaders.ContainsKey("Authorization"))
                return this;

            customHeaders.Remove("Authorization");
            return this;
        }

        public Task<HttpResponse> Delete(string url, object payload = null)
        {
            return DoRequest("DELETE", url, payload);
        }

        public Task<HttpResponse<T>> DeleteJson<T>(string url, object payload = null)
        {
            return DoRequest<T>("DELETE", url, payload);
        }

        public Task<HttpResponse> Get(string url)
        {
            return DoRequest("GET", url);
        }

        public Task<HttpResponse<T>> GetJson<T>(string url)
        {
            return DoRequest<T>("GET", url);
        }

        public Task<HttpResponse<Blob>> GetBlob(string url, Action<decimal> onProgress = null)
        {
            TaskCompletionSource<HttpResponse<Blob>> taskCompletionSource = new TaskCompletionSource<HttpResponse<Blob>>();

            XMLHttpRequest xhr = new XMLHttpRequest();

            xhr.OnReadyStateChange = () =>
            {
                if (xhr.ReadyState != AjaxReadyState.Done)
                {
                    return;
                }
                if (xhr.Status < 200 || xhr.Status >= 300)
                {
                    taskCompletionSource.SetResult(new HttpResponse<Blob>(null, xhr.Status, $"Status: {xhr.StatusText}; Content: {xhr.ResponseText}"));
                    return;
                }

                taskCompletionSource.SetResult(new HttpResponse<Blob>(xhr.Response.As<Blob>(), xhr.Status));
            };
            xhr.OnProgress = progressEvent => onProgress?.Invoke(progressEvent.Loaded / (decimal)progressEvent.Total);

            xhr.Open("GET", url);
            foreach (KeyValuePair<string, string> header in customHeaders)
            {
                xhr.SetRequestHeader(header.Key, header.Value);
            }
            xhr.ResponseType = XMLHttpRequestResponseType.Blob;
            xhr.Send();

            return taskCompletionSource.Task;
        }

        public Task<HttpResponse> Post(string url, object payload = null)
        {
            return DoRequest("POST", url, payload);
        }

        public Task<HttpResponse<T>> PostJson<T>(string url, object payload = null)
        {
            return DoRequest<T>("POST", url, payload);
        }

        public Task<HttpResponse> PostMultipartFormData(string url, FormData formData, Action<decimal> onProgress = null)
        {
            return
                DoRequest("POST", url, formData, "multipart/form-data", onProgress);
        }

        public Task<HttpResponse> Put(string url, object payload = null)
        {
            return DoRequest("PUT", url, payload);
        }

        private Task<HttpResponse> DoRequest(string method, string url, object payload = null, string mimeType = "application/json", Action<decimal> onProgress = null)
        {
            AjaxOptions options = GetAjaxOptions(url, mimeType, onProgress);
            options.Type = method;
            if (mimeType == "application/json")
                options.Data = JsonConvert.SerializeObject(payload);
            else
                options.Data = payload;

            TaskCompletionSource<HttpResponse> taskCompletionSource = new TaskCompletionSource<HttpResponse>();

            jQuery
                .Ajax(url, options)
                .Done((data, status, xhr) => taskCompletionSource.SetResult(ParseResultSuccess(xhr)))
                .Fail((xhr, status, error) => taskCompletionSource.SetResult(ParseResultFailure(xhr, error)))
                ;

            return taskCompletionSource.Task;
        }

        private Task<HttpResponse<T>> DoRequest<T>(string method, string url, object payload = null, string mimeType = "application/json")
        {
            AjaxOptions options = GetAjaxOptions(url, mimeType);
            options.Type = method;
            options.Data = JsonConvert.SerializeObject(payload);

            TaskCompletionSource<HttpResponse<T>> taskCompletionSource = new TaskCompletionSource<HttpResponse<T>>();

            jQuery
                .Ajax(url, options)
                .Done((data, status, xhr) => taskCompletionSource.SetResult(ParseResultSuccess<T>(xhr)))
                .Fail((xhr, status, error) => taskCompletionSource.SetResult(ParseResultFailure<T>(xhr, error)))
                ;

            return taskCompletionSource.Task;
        }

        private static HttpResponse<T> ParseResultSuccess<T>(jqXHR xhr)
        {
            return new HttpResponse<T>(ParseAjaxPayload<T>(xhr), xhr.Status) { Content = xhr["responseText"]?.ToString() };
        }

        private static HttpResponse<T> ParseResultFailure<T>(jqXHR xhr, string error)
        {
            if (xhr.Status >= 200 && xhr.Status < 300)
                return ParseResultSuccess<T>(xhr);

            if (xhr.Status == 404) //Not found
            {
                error = "Endpoint not found";
                return new HttpResponse<T>(default(T), xhr.Status, error) { IsUnauthorized = true };
            }

            if (xhr.Status == 401) //Unauthorised
            {
                error = "Access Denied";
                return new HttpResponse<T>(default(T), xhr.Status, error) { IsUnauthorized = true };
            }
            else if (xhr.Status == 403) //Forbidden
            {
                dynamic payload = ParseAjaxPayload<dynamic>(xhr);

                if (payload != null && payload.Code == 401)
                    return new HttpResponse<T>(default(T), xhr.Status, "Access Denied");

                if (payload != null && !string.IsNullOrEmpty(payload.Location))
                    Window.Location.Href = payload.Location;

                return new HttpResponse<T>(default(T), xhr.Status, error) { SessionExpired = true };
            }

            string parsedError = ParseAjaxPayload<string>(xhr);
            if (!string.IsNullOrEmpty(parsedError))
                return new HttpResponse<T>(default(T), xhr.Status, parsedError) { Content = xhr["responseText"]?.ToString() };

            return new HttpResponse<T>(ParseAjaxPayload<T>(xhr), xhr.Status, error) { Content = xhr["responseText"]?.ToString() };
        }

        private static HttpResponse ParseResultSuccess(jqXHR xhr)
        {
            return new HttpResponse(xhr.Status) { Content = xhr["responseText"]?.ToString() };
        }

        private static HttpResponse ParseResultFailure(jqXHR xhr, string error)
        {
            if (xhr.Status >= 200 && xhr.Status < 300)
                return ParseResultSuccess(xhr);

            if (xhr.Status == 404) //Not found
            {
                error = "Endpoint not found";
                return new HttpResponse(xhr.Status, error) { IsUnauthorized = true };
            }
            if (xhr.Status == 401) //Unauthorised
            {
                error = "Access Denied";
                return new HttpResponse(xhr.Status, error) { IsUnauthorized = true };
            }
            else if (xhr.Status == 403) //Forbidden
            {
                dynamic response = xhr.Response;

                if (response != null && !string.IsNullOrEmpty(response.Code))
                    return new HttpResponse(xhr.Status, "Access Denied") { SessionExpired = true };

                if (response != null && !string.IsNullOrEmpty(response.Location))
                    Window.Location.Href = response.Location;

                return new HttpResponse(xhr.Status, error) { SessionExpired = true };
            }

            string parsedError = ParseAjaxPayload<string>(xhr);
            if (!string.IsNullOrEmpty(parsedError))
                error = parsedError;

            return new HttpResponse(xhr.Status, error) { Content = xhr["responseText"]?.ToString() };
        }

        private static T ParseAjaxPayload<T>(jqXHR xhr)
        {
            try
            {
                string response = xhr["responseText"]?.ToString();

                if (string.IsNullOrEmpty(response))
                    return default(T);

                bool isJson = xhr["responseJSON"] != null;

                if (!isJson)
                    response = $"\"{response}\"";

                //return JSON.Parse<T>(RemoveTypeHandling(response), (prop, val) =>
                //{
                //    if (val != null && isoDateRegEx.Test(val.ToString()))
                //    {
                //        object boxedDate = DateTime.Parse((string)val.ValueOf());
                //        return boxedDate.ToDynamic().v;
                //    }
                //    return val;
                //});


                // The Bridge version of Json.NET cannot deserialize nullable Guid
                if (typeof(T) == typeof(Guid?))
                {
                    Guid result;
                    if (!Guid.TryParse(response.Trim('"'), out result))
                        return default(T);

                    return (dynamic)result;
                }

                return JsonConvert.DeserializeObject<T>(RemoveTypeHandling(response), new JsonSerializerSettings
                {
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                });
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        //"\$type"\s*:\s*"[a-zA-Z0-9_]+"
        private static readonly RegExp typeHandlerRegEx = new RegExp("\"\\$type\"\\s*:\\s*\"[a-zA-Z0-9_]+\"\\,?", "gi");
        private static string RemoveTypeHandling(string jsonString)
        {
            return jsonString.Replace(typeHandlerRegEx, string.Empty);
        }

        private AjaxOptions GetAjaxOptions(string url, string mimeType = "application/json", Action<decimal> onProgress = null)
        {
            AjaxOptions options = new AjaxOptions();

            options.Cache = false;
            if (mimeType == "multipart/form-data")
            {
                options.ContentType = false;
                options.ProcessData = false;
                options.Xhr = () =>
                {
                    var xhr = new XMLHttpRequest();

                    if (xhr.Upload == null)
                        return xhr;

                    xhr.Upload.AddEventListener(EventType.Progress, ev =>
                    {
                        ProgressEvent progressEvent = ev.As<ProgressEvent>();
                        if (progressEvent != null && onProgress != null)
                        {
                            onProgress.Invoke(progressEvent.Loaded / (decimal)progressEvent.Total);
                        }

                    }, false);

                    return xhr;
                };
            }
            else
            {
                options.ContentType = mimeType + "; charset=utf-8";
            }
            if (mimeType == "application/json")
                options.DataType = "json";
            options.Headers = GetHeaders();
            options.Url = FormatUrl(url);
            return options;
        }

        private string FormatUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            if (!url.StartsWith("/"))
                url = "/" + url;

            return url;
        }

        private object GetHeaders()
        {
            //HTMLInputElement input = (HTMLInputElement)Document.GetElementsByName("__RequestVerificationToken")[0];

            //string timeZoneIntl = null;

            //if (Window.ToDynamic().Intl != null)
            //    timeZoneIntl = Window.ToDynamic().Intl.DateTimeFormat().resolvedOptions().timeZone;

            object headers = new object();

            headers["__TimeZone"] = new Date().GetTimezoneOffset().ToString();
            headers["__ClientCulture"] = Window.Navigator.Language;

            foreach (KeyValuePair<string, string> customHeader in customHeaders)
            {
                if (string.IsNullOrWhiteSpace(customHeader.Key)
                    || string.IsNullOrWhiteSpace(customHeader.Value))
                    continue;

                headers[customHeader.Key] = customHeader.Value;
            }

            return headers;
        }
    }

    public class HttpResponse
    {
        public HttpResponse(int statusCode, string error = null)
        {
            StatusCode = statusCode;
            Error = error;
        }

        public bool IsSuccessful
        {
            get
            {
                return StatusCode >= 200 && StatusCode < 300 && string.IsNullOrWhiteSpace(Error);
            }
        }

        public int StatusCode { get; set; }

        public string Error { get; set; }

        public bool IsUnauthorized { get; set; }

        public bool SessionExpired { get; set; }

        public string Content { get; set; }

        public HttpResponse<T> TypeTo<T>()
        {
            return new HttpResponse<T>(
                Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Content, new JsonSerializerSettings
                {
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                }),
                StatusCode,
                Error
                )
            {
                IsUnauthorized = IsUnauthorized,
                SessionExpired = SessionExpired,
                Content = Content,
            };
        }
    }

    public class HttpResponse<TPayload> : HttpResponse
    {
        public HttpResponse(TPayload payload, int statusCode, string error = null)
            : base(statusCode, error)
        {
            Payload = payload;
        }

        public TPayload Payload { get; set; }
    }
}
