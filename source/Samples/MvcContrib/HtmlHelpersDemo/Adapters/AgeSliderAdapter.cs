using Griffin.MvcContrib.Html;

namespace HtmlHelpersDemo.Adapters
{
    public class AgeSliderAdapter : IFormItemAdapter
    {
        #region IFormItemAdapter Members

        /// <summary>
        /// Process a tag
        /// </summary>
        /// <param name="context">Context with all html tag information</param>
        public void Process(FormItemAdapterContext context)
        {
            if (context.Metadata.PropertyName != "Age")
                return;

            context.TagBuilder.MergeAttribute("disabled", "disabled");


            var scriptTag = new NestedTagBuilder("script");
            scriptTag.MergeAttribute("type", "text/javascript");
            scriptTag.InnerHtml = string.Format(
                @"$(function(){{ 
    var $slider = $('<div id=""slider""></div>').insertAfter($('#{0}'));
    $slider.slider({{
        min: 0, 
        max:110, 
        value: {1},
		slide: function( event, ui ) {{
			$('#{0}').val(ui.value);
        }}

    }});
}});",
                context.TagBuilder.Attributes["id"], context.TagBuilder.Attributes["value"]);

            var div = new NestedTagBuilder("div");
            div.AddChild(context.TagBuilder);
            div.AddChild(scriptTag);
            context.TagBuilder = div;
        }

        #endregion
    }
}