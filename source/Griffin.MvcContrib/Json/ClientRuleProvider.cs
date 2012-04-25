using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Json
{
    /// <summary>
    /// Generates a JSON string from all client validation rules
    /// </summary>
    public class ClientRuleProvider
    {
        /// <summary>
        /// Get validation rules for an model
        /// </summary>
        /// <param name="controllerContext">Current controller context</param>
        /// <param name="model">Model to validate</param>
        /// <returns>Object which can be JSON serialized</returns>
        /// <remarks>Will only return rules which has not been fulfilled.</remarks>
        public ValidationRules GetRules(ControllerContext controllerContext, object model)
        {
            var metadata2 = ModelMetadataProviders.Current.GetMetadataForProperties(model, model.GetType()  );
            var validation = new ValidationRules();
            foreach (var modelMetadata in metadata2)
            {
                var validators = modelMetadata.GetValidators(controllerContext);
                if (!validators.Any())
                    continue;

                foreach (var validator in validators)
                {
                    foreach (var rule in validator.GetClientValidationRules())
                    {
                        if (rule.ValidationParameters.Any())
                        {
                            foreach (var kvp in rule.ValidationParameters)
                            {
                                validation.Rules.Add(modelMetadata.PropertyName, kvp.Key, kvp.Value.ToString());
                            }
                        }
                        else
                            validation.Rules.Add(modelMetadata.PropertyName, rule.ValidationType, "true");

                        validation.Messages.Add(modelMetadata.PropertyName, rule.ValidationType, rule.ErrorMessage);
                    }
                }
            }

            return validation;
        }

    }
}
