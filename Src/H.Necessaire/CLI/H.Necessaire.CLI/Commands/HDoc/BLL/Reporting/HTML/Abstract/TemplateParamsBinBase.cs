using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Abstract
{
    internal abstract class TemplateParamsBinBase : ImATemplateParamsBin, ImATemplateProcessor
    {
        readonly Lazy<PropertyInfo[]> templateParamPropertiesThatReturnString;
        readonly Lazy<PropertyInfo[]> templateParamPropertiesThatReturnImATemplateParam;
        readonly Lazy<MethodInfo[]> templateParamMethodsThatReturnStringTask;
        readonly Lazy<MethodInfo[]> templateParamMethodsThatReturnImATemplateParamTask;
        private ImATemplateParam[] parsedTemplateParams = null;
        readonly SemaphoreSlim templateParamsParseSemaphore = new SemaphoreSlim(1, 1);
        protected TemplateParamsBinBase()
        {
            templateParamPropertiesThatReturnString = new Lazy<PropertyInfo[]>(ParseTemplateParamPropertiesThatReturnString);
            templateParamPropertiesThatReturnImATemplateParam = new Lazy<PropertyInfo[]>(ParseTemplateParamPropertiesThatReturnImATemplateParam);
            templateParamMethodsThatReturnStringTask = new Lazy<MethodInfo[]>(ParseTemplateParamMethodsThatReturnStringTask);
            templateParamMethodsThatReturnImATemplateParamTask = new Lazy<MethodInfo[]>(ParseTemplateParamMethodsThatReturnImATemplateParamTask);
        }

        public virtual async Task<IEnumerable<ImATemplateParam>> ReadParams()
        {
            await EnsureTemplateParamsParsing();

            return parsedTemplateParams;
        }

        public virtual async Task<string> Process(string template, IEnumerable<ImATemplateParam> templateParams)
        {
            string result = template;
            foreach(ImATemplateParam templateParam in templateParams)
            {
                result = result.Replace($"{{{{{templateParam.ID}}}}}", await templateParam.Read());
            }
            return result;
        }

        protected async Task EnsureTemplateParamsParsing()
        {
            if (parsedTemplateParams != null)
                return;

            await templateParamsParseSemaphore.WaitAsync(TimeSpan.FromSeconds(4));

            if (parsedTemplateParams != null)
            {
                templateParamsParseSemaphore.Release();
                return;
            }

            await
                new Func<Task>(async () =>
                {

                    ImATemplateParam[] templateParamFromMethodsThatReturnImATemplateParamTask = Array.Empty<ImATemplateParam>();
                    if (templateParamMethodsThatReturnImATemplateParamTask.Value.Any())
                    {
                        templateParamFromMethodsThatReturnImATemplateParamTask
                            = await Task.WhenAll(
                                templateParamMethodsThatReturnImATemplateParamTask.Value
                                .Select(m => (Task<ImATemplateParam>)m.Invoke(this, Array.Empty<object>()))
                            );
                    }

                    parsedTemplateParams =
                        templateParamPropertiesThatReturnString.Value.Select(p => new StaticTemplateParam(p.Name, (string)p.GetValue(this)) as ImATemplateParam)
                        .Union(
                            templateParamPropertiesThatReturnImATemplateParam.Value.Select(p => (ImATemplateParam)p.GetValue(this))
                        )
                        .Union(
                            templateParamMethodsThatReturnStringTask.Value.Select(m => new TemplateParam(m.Name, async () => await (Task<string>)m.Invoke(this, Array.Empty<object>())))
                        )
                        .Union(
                            templateParamFromMethodsThatReturnImATemplateParamTask
                        )
                        .ToArray()
                        ;

                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        templateParamsParseSemaphore.Release();
                    }
                );

            templateParamsParseSemaphore.Release();
        }

        private PropertyInfo[] ParseTemplateParamPropertiesThatReturnString()
        {
            return
                this
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && typeof(string).IsAssignableFrom(p.PropertyType))
                .ToArray()
                ;
        }

        private PropertyInfo[] ParseTemplateParamPropertiesThatReturnImATemplateParam()
        {
            return
                this
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && typeof(ImATemplateParam).IsAssignableFrom(p.PropertyType))
                .ToArray()
                ;
        }

        private MethodInfo[] ParseTemplateParamMethodsThatReturnStringTask()
        {
            return
                this
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => !m.GetParameters().Any() && typeof(Task<string>).IsAssignableFrom(m.ReturnType))
                .ToArray()
                ;
        }

        private MethodInfo[] ParseTemplateParamMethodsThatReturnImATemplateParamTask()
        {
            return
                this
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => !m.GetParameters().Any() && typeof(Task<ImATemplateParam>).IsAssignableFrom(m.ReturnType))
                .ToArray()
                ;
        }
    }
}
