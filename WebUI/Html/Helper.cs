using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ERP.WebUI.Html
{
    public static class Helper
    {
        public static HtmlString SimpleDropDown(this HtmlHelper helper, string htmlFieldName)
        {
            string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            string str = HttpContext.Current.Request.Params[fullHtmlFieldName];
            TagBuilder tagBuilder = new TagBuilder("select");
            tagBuilder.Attributes.Add("id", fullHtmlFieldName);
            tagBuilder.Attributes.Add("name", fullHtmlFieldName);
            if (str != null)
                tagBuilder.Attributes.Add("data-selected", str);
            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        public static HtmlString SimpleDropDown(
          this HtmlHelper helper,
          string htmlFieldName,
          string dataUrl)
        {
            return helper.SimpleDropDown(htmlFieldName, dataUrl, null);
        }

        public static HtmlString SimpleDropDown(
          this HtmlHelper helper,
          string htmlFieldName,
          string dataUrl,
          object htmlAttributes)
        {
            string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            string str = HttpContext.Current.Request.Params[fullHtmlFieldName];
            TagBuilder tagBuilder = new TagBuilder("select");
            tagBuilder.Attributes.Add("id", fullHtmlFieldName);
            tagBuilder.Attributes.Add("name", fullHtmlFieldName);
            tagBuilder.Attributes.Add("data-url", dataUrl);
            if (htmlAttributes != null)
            {
                RouteValueDictionary htmlAttributes1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tagBuilder.MergeAttributes(htmlAttributes1);
            }
            tagBuilder.AddCssClass("Dropdown");
            if (str != null)
                tagBuilder.Attributes.Add("data-selected", str);
            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        public static HtmlString SimpleDropDownFor<TModel, TValue>(
          this HtmlHelper<TModel> helper,
          Expression<Func<TModel, TValue>> expression)
        {
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            TagBuilder tagBuilder = new TagBuilder("select");
            tagBuilder.Attributes.Add("id", fullHtmlFieldName);
            tagBuilder.Attributes.Add("name", fullHtmlFieldName);
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            if (metadata.Model != null)
                tagBuilder.Attributes.Add("data-selected", metadata.Model.ToString());
            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.Errors.Count > 0)
                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            tagBuilder.MergeAttributes(helper.GetUnobtrusiveValidationAttributes(expressionText, metadata));
            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        public static HtmlString SimpleDropDownFor<TModel, TValue>(
          this HtmlHelper<TModel> helper,
          Expression<Func<TModel, TValue>> expression,
          object htmlAttributes)
        {
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            TagBuilder tagBuilder = new TagBuilder("select");
            tagBuilder.Attributes.Add("id", fullHtmlFieldName);
            tagBuilder.Attributes.Add("name", fullHtmlFieldName);
            if (htmlAttributes != null)
            {
                RouteValueDictionary htmlAttributes1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tagBuilder.MergeAttributes(htmlAttributes1);
            }
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            if (metadata.Model != null)
                tagBuilder.Attributes.Add("data-selected", metadata.Model.ToString());
            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.Errors.Count > 0)
                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            tagBuilder.MergeAttributes(helper.GetUnobtrusiveValidationAttributes(expressionText, metadata));
            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        public static HtmlString SimpleDropDownFor<TModel, TValue>(
          this HtmlHelper<TModel> helper,
          Expression<Func<TModel, TValue>> expression,
          string dataUrl)
        {
            return helper.SimpleDropDownFor(expression, dataUrl, null);
        }

        public static HtmlString SimpleDropDownFor<TModel, TValue>(
          this HtmlHelper<TModel> helper,
          Expression<Func<TModel, TValue>> expression,
          string dataUrl,
          object htmlAttributes)
        {
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            TagBuilder tagBuilder = new TagBuilder("select");
            tagBuilder.Attributes.Add("id", fullHtmlFieldName);
            tagBuilder.Attributes.Add("name", fullHtmlFieldName);
            tagBuilder.Attributes.Add("data-url", dataUrl);
            if (htmlAttributes != null)
            {
                RouteValueDictionary htmlAttributes1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tagBuilder.MergeAttributes(htmlAttributes1);
            }
            tagBuilder.AddCssClass("Dropdown");
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            if (metadata.Model != null)
                tagBuilder.Attributes.Add("data-selected", metadata.Model.ToString());
            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.Errors.Count > 0)
                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            tagBuilder.MergeAttributes(helper.GetUnobtrusiveValidationAttributes(expressionText, metadata));
            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        public static HtmlString CascadingDropDown(
          this HtmlHelper helper,
          string htmlFieldName,
          string dataUrl,
          string parent)
        {
            return helper.CascadingDropDown(htmlFieldName, dataUrl, parent, null);
        }

        public static HtmlString CascadingDropDown(
          this HtmlHelper helper,
          string htmlFieldName,
          string dataUrl,
          string parent,
          object htmlAttributes)
        {
            string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            string str = HttpContext.Current.Request.Params[fullHtmlFieldName];
            TagBuilder tagBuilder = new TagBuilder("select");
            tagBuilder.Attributes.Add("id", fullHtmlFieldName);
            tagBuilder.Attributes.Add("name", fullHtmlFieldName);
            tagBuilder.Attributes.Add("data-url", dataUrl);
            tagBuilder.Attributes.Add("data-parent", parent);
            if (htmlAttributes != null)
            {
                RouteValueDictionary htmlAttributes1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tagBuilder.MergeAttributes(htmlAttributes1);
            }
            tagBuilder.AddCssClass("Cascading");
            if (str != null)
                tagBuilder.Attributes.Add("data-selected", str);
            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        public static HtmlString CascadingDropDownFor<TModel, TValue>(
          this HtmlHelper<TModel> helper,
          Expression<Func<TModel, TValue>> expression,
          string dataUrl,
          string parent)
        {
            return helper.CascadingDropDownFor(expression, dataUrl, parent, null);
        }

        public static HtmlString CascadingDropDownFor<TModel, TValue>(
          this HtmlHelper<TModel> helper,
          Expression<Func<TModel, TValue>> expression,
          string dataUrl,
          string parent,
          object htmlAttributes)
        {
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            TagBuilder tagBuilder = new TagBuilder("select");
            tagBuilder.Attributes.Add("id", fullHtmlFieldName);
            tagBuilder.Attributes.Add("name", fullHtmlFieldName);
            tagBuilder.Attributes.Add("data-url", dataUrl);
            tagBuilder.Attributes.Add("data-parent", parent);
            if (htmlAttributes != null)
            {
                RouteValueDictionary htmlAttributes1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tagBuilder.MergeAttributes(htmlAttributes1);
            }
            tagBuilder.AddCssClass("Cascading");
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            if (metadata.Model != null)
                tagBuilder.Attributes.Add("data-selected", metadata.Model.ToString());
            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.Errors.Count > 0)
                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            tagBuilder.MergeAttributes(helper.GetUnobtrusiveValidationAttributes(expressionText, metadata));
            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        public static HtmlString CascadingDropDownFor<TModel, TValue>(
          this HtmlHelper<TModel> helper,
          Expression<Func<TModel, TValue>> expression,
          string dataUrl,
          string parent,
          string parent2,
          object htmlAttributes)
        {
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            TagBuilder tagBuilder = new TagBuilder("select");
            tagBuilder.Attributes.Add("id", fullHtmlFieldName);
            tagBuilder.Attributes.Add("name", fullHtmlFieldName);
            tagBuilder.Attributes.Add("data-url", dataUrl);
            tagBuilder.Attributes.Add("data-parent", parent);
            tagBuilder.Attributes.Add("data-parent2", parent2);
            if (htmlAttributes != null)
            {
                RouteValueDictionary htmlAttributes1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tagBuilder.MergeAttributes(htmlAttributes1);
            }
            tagBuilder.AddCssClass("Cascading");
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            if (metadata.Model != null)
                tagBuilder.Attributes.Add("data-selected", metadata.Model.ToString());
            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.Errors.Count > 0)
                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            tagBuilder.MergeAttributes(helper.GetUnobtrusiveValidationAttributes(expressionText, metadata));
            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        public static HtmlString CascadingDropDownFor<TModel, TValue>(
          this HtmlHelper<TModel> helper,
          Expression<Func<TModel, TValue>> expression,
          string dataUrl,
          string parent,
          string parent2,
          string parent3,
          object htmlAttributes)
        {
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            TagBuilder tagBuilder = new TagBuilder("select");
            tagBuilder.Attributes.Add("id", fullHtmlFieldName);
            tagBuilder.Attributes.Add("name", fullHtmlFieldName);
            tagBuilder.Attributes.Add("data-url", dataUrl);
            tagBuilder.Attributes.Add("data-parent", parent);
            tagBuilder.Attributes.Add("data-parent2", parent2);
            tagBuilder.Attributes.Add("data-parent3", parent3);
            if (htmlAttributes != null)
            {
                RouteValueDictionary htmlAttributes1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tagBuilder.MergeAttributes(htmlAttributes1);
            }
            tagBuilder.AddCssClass("Cascading");
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            if (metadata.Model != null)
                tagBuilder.Attributes.Add("data-selected", metadata.Model.ToString());
            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.Errors.Count > 0)
                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            tagBuilder.MergeAttributes(helper.GetUnobtrusiveValidationAttributes(expressionText, metadata));
            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        public static HtmlString DatePicker(this HtmlHelper helper, string htmlFieldName)
        {
            return helper.DatePicker(htmlFieldName, null);
        }

        public static HtmlString DatePicker(
          this HtmlHelper helper,
          string htmlFieldName,
          object htmlAttributes)
        {
            string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            string str = HttpContext.Current.Request.Form[fullHtmlFieldName];
            TagBuilder tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes.Add("type", "text");
            tagBuilder.Attributes.Add("id", fullHtmlFieldName);
            tagBuilder.Attributes.Add("name", fullHtmlFieldName);
            if (htmlAttributes != null)
            {
                RouteValueDictionary htmlAttributes1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tagBuilder.MergeAttributes(htmlAttributes1);
            }
            tagBuilder.AddCssClass(nameof(DatePicker));
            if (str != null)
            {
                try
                {
                    DateTime dateTime = Convert.ToDateTime(str);
                    if (dateTime != new DateTime())
                        tagBuilder.Attributes.Add("value", dateTime.ToString("dd-MMM-yyyy"));
                }
                catch (Exception ex)
                {
                    tagBuilder.Attributes.Add("value", "");
                }
            }
            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

        public static HtmlString DatePickerFor<TModel, TValue>(
          this HtmlHelper<TModel> helper,
          Expression<Func<TModel, TValue>> expression)
        {
            return helper.DatePickerFor(expression, null);
        }

        public static HtmlString DatePickerFor<TModel, TValue>(
          this HtmlHelper<TModel> helper,
          Expression<Func<TModel, TValue>> expression,
          object htmlAttributes)
        {
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            TagBuilder tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes.Add("type", "text");
            tagBuilder.Attributes.Add("id", fullHtmlFieldName);
            tagBuilder.Attributes.Add("name", fullHtmlFieldName);
            if (htmlAttributes != null)
            {
                RouteValueDictionary htmlAttributes1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tagBuilder.MergeAttributes(htmlAttributes1);
            }
            tagBuilder.AddCssClass("DatePicker");
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            if (metadata.Model != null)
            {
                try
                {
                    DateTime dateTime = Convert.ToDateTime(metadata.Model);
                    if (dateTime != new DateTime())
                        tagBuilder.Attributes.Add("value", dateTime.ToString("dd-MMM-yyyy"));
                }
                catch (Exception ex)
                {
                    tagBuilder.Attributes.Add("value", "");
                }
            }
            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.Errors.Count > 0)
                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            tagBuilder.MergeAttributes(helper.GetUnobtrusiveValidationAttributes(expressionText, metadata));
            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

        public static HtmlString TimePicker(this HtmlHelper helper, string htmlFieldName)
        {
            return helper.TimePicker(htmlFieldName, null);
        }

        public static HtmlString TimePicker(
          this HtmlHelper helper,
          string htmlFieldName,
          object htmlAttributes)
        {
            string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            string str = HttpContext.Current.Request.Form[fullHtmlFieldName];
            TagBuilder tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes.Add("type", "text");
            tagBuilder.Attributes.Add("id", fullHtmlFieldName);
            tagBuilder.Attributes.Add("name", fullHtmlFieldName);
            if (htmlAttributes != null)
            {
                RouteValueDictionary htmlAttributes1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tagBuilder.MergeAttributes(htmlAttributes1);
            }
            tagBuilder.AddCssClass(nameof(TimePicker));
            if (str != null)
            {
                try
                {
                    tagBuilder.Attributes.Add("value", str);
                }
                catch (Exception ex)
                {
                    tagBuilder.Attributes.Add("value", "");
                }
            }
            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

        public static HtmlString TimePickerFor<TModel, TValue>(
          this HtmlHelper<TModel> helper,
          Expression<Func<TModel, TValue>> expression)
        {
            return helper.TimePickerFor(expression, null);
        }

        public static HtmlString TimePickerFor<TModel, TValue>(
          this HtmlHelper<TModel> helper,
          Expression<Func<TModel, TValue>> expression,
          object htmlAttributes)
        {
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            TagBuilder tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes.Add("type", "text");
            tagBuilder.Attributes.Add("id", fullHtmlFieldName);
            tagBuilder.Attributes.Add("name", fullHtmlFieldName);
            if (htmlAttributes != null)
            {
                RouteValueDictionary htmlAttributes1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tagBuilder.MergeAttributes(htmlAttributes1);
            }
            tagBuilder.AddCssClass("TimePicker");
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            if (metadata.Model != null)
            {
                try
                {
                    tagBuilder.Attributes.Add("value", metadata.Model.ToString());
                }
                catch (Exception ex)
                {
                    tagBuilder.Attributes.Add("value", "");
                }
            }
            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.Errors.Count > 0)
                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            tagBuilder.MergeAttributes(helper.GetUnobtrusiveValidationAttributes(expressionText, metadata));
            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

        public static HtmlString YesNoFor<TModel, TValue>(
          this HtmlHelper<TModel> helper,
          Expression<Func<TModel, TValue>> expression)
        {
            ModelMetadata modelMetadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            string str1 = modelMetadata.DisplayName;
            if (str1 == null)
            {
                string propertyName = modelMetadata.PropertyName;
                if (propertyName == null)
                    str1 = ((IEnumerable<string>)expressionText.Split('.')).Last();
                else
                    str1 = propertyName;
            }
            string str2 = str1;
            TagBuilder tagBuilder = new TagBuilder("label");
            tagBuilder.Attributes.Add("title", str2);
            tagBuilder.Attributes.Add("id", str2);
            if (modelMetadata.Model != null)
                tagBuilder.InnerHtml = Convert.ToBoolean(modelMetadata.Model) ? "Active" : "Inactive";
            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        public static HtmlString YesNoFor<TModel, TValue>(
          this HtmlHelper<TModel> helper,
          Expression<Func<TModel, TValue>> expression,
          object htmlAttributes)
        {
            ModelMetadata modelMetadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            string str1 = modelMetadata.DisplayName;
            if (str1 == null)
            {
                string propertyName = modelMetadata.PropertyName;
                if (propertyName == null)
                    str1 = ((IEnumerable<string>)expressionText.Split('.')).Last();
                else
                    str1 = propertyName;
            }
            string str2 = str1;
            TagBuilder tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes.Add("type", "text");
            tagBuilder.Attributes.Add("title", str2);
            tagBuilder.Attributes.Add("id", str2);
            tagBuilder.Attributes.Add("name", str2);
            if (htmlAttributes != null)
            {
                RouteValueDictionary htmlAttributes1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tagBuilder.MergeAttributes(htmlAttributes1);
            }
            if (modelMetadata.Model != null)
                tagBuilder.Attributes.Add("Value", Convert.ToBoolean(modelMetadata.Model) ? "Active" : "Inactive");
            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

        public static HtmlString StatusFor<TModel, TValue>(
          this HtmlHelper<TModel> helper,
          Expression<Func<TModel, TValue>> expression)
        {
            ModelMetadata modelMetadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            string str = string.Empty;
            if (modelMetadata.Model != null)
                str = Convert.ToBoolean(modelMetadata.Model) ? "Active" : "Inactive";
            return MvcHtmlString.Create(str);
        }
    }
}