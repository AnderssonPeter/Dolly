using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dolly
{
    /// <summary>
    /// Provides functionality to convert a <see cref="Model"/> into a <see cref="SourceText"/> representation.
    /// </summary>
    public class SourceTextConverter
    {
        /// <summary>
        /// Converts the given <see cref="Model"/> into a <see cref="SourceText"/> object.
        /// </summary>
        /// <param name="model">The model to convert into source text.</param>
        /// <returns>A <see cref="SourceText"/> object representing the generated source code for the model.</returns>
        public static SourceText ToSourceText(Model model)
        {
            return SourceText.From($$"""
                    using global::System.Linq;
                    namespace {{model.Namespace}};
                    {{model.GetModifiers()}} {{model.Name}} : global::Dolly.IClonable<{{model.Name}}>
                    {
                        {{(!model.HasClonableBaseClass ? "object global::System.ICloneable.Clone() => this.DeepClone();" : "")}}
                        public {{model.GetMethodModifiers()}}global::{{model.Namespace}}.{{model.Name}} DeepClone() =>
                            new ({{string.Join(", ", model.Constructor.Select(m => m.ToString(true)))}})
                            {
                    {{GenerateCloneMembers(model, true)}}
                            };

                        public {{model.GetMethodModifiers()}}global::{{model.Namespace}}.{{model.Name}} ShallowClone() =>
                            new ({{string.Join(", ", model.Constructor.Select(m => m.ToString(false)))}})
                            {
                    {{GenerateCloneMembers(model, false)}}
                            };
                    }
                    """.Replace("\r\n", "\n"), Encoding.UTF8);
        }

        /// <summary>
        /// Generates the member initialization code for cloning operations.
        /// </summary>
        /// <param name="model">The model containing the members to clone.</param>
        /// <param name="deepClone">Indicates whether to generate deep clone or shallow clone member initializations.</param>
        /// <returns>A string containing the member initialization code.</returns>
        private static string GenerateCloneMembers(Model model, bool deepClone) =>
            string.Join(",\n",
                model.Members
                .Select(m => $"{new string(' ', 12)}{m.Name} = {m.ToString(deepClone)}")
            );
    }
}
