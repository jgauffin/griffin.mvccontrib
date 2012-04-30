using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Griffin.MvcContrib.Json;
using Newtonsoft.Json;
using Xunit;
//using ModelError = Griffin.MvcContrib.Json.ModelError;

namespace Griffin.MvcContrib.Tests.Json
{
    public class JsonResponseTests
    {
        [Fact]
        public void SerializeError()
        {
            var response = new JsonResponse(false, new ErrorMessage("Something failed"));

            var actual = JsonConvert.SerializeObject(response);

            Assert.Equal(@"{""success"":false,""contentType"":""error"",""body"":{""message"":""Something failed""}}", actual);
        }

        [Fact]
        public void SerializeModelError()
        {
            var errors = new List<KeyValuePair<string, ModelState>>();
            var state = new ModelState();
            state.Errors.Add("some error");
            errors.Add(new KeyValuePair<string, ModelState>("Something", state));
            var modelError = new ModelStateJson(errors);
            var response = new JsonResponse(false, modelError);

            var actual = JsonConvert.SerializeObject(response);

            Assert.Equal(@"{""success"":false,""contentType"":""model-errors"",""body"":{""Something"":[""some error""]}}", actual);
        }

        [Fact]
        public void SerializeValidationRules()
        {
            var rules = new ValidationRules();
            rules.Messages.Add("SomeProp", "required", "The field is required, dude!");
            rules.Rules.Add("someProp", "required", "true");
            rules.Rules.Add("someProp", "max", "40");

            var actual = JsonConvert.SerializeObject(new JsonResponse(true, rules));

            Assert.Equal(@"{""success"":true,""contentType"":""validation-rules"",""body"":{""messages"":{""SomeProp"":{""required"":""The field is required, dude!""}},""rules"":{""someProp"":{""required"":""true""}}}}", actual);
        }
    }
}
