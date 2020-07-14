namespace Carter.App.MetaData.Extra
{
    using Carter.OpenApi;

    /// <summary>
    /// Helper to avoid repetition while documenting.
    /// </summary>
    public class MetaDataExtra
    {
        /// <summary>
        /// Gets documentation for the ?bson query string option.
        /// </summary>
        /// <returns>
        /// Documentation.
        /// </returns>
        public static QueryStringParameter GetBsonDocumentation()
        {
            return new QueryStringParameter
            {
                Name = "bson",
                Description = "When true, encode the responce in BSON format.",
                Type = typeof(bool),
            };
        }

        /// <summary>
        /// Gets documentation for the ?ugly query string option.
        /// </summary>
        /// <returns>
        /// Documentation.
        /// </returns>
        public static QueryStringParameter GetUglyDocumentation()
        {
            return new QueryStringParameter
            {
                Name = "ugly",
                Description = "When true, strip whitespace from the JSON responce. "
                    + "Uses less resources. "
                    + "Nothing happens with ?bson=true.",
                Type = typeof(bool),
            };
        }
    }
}