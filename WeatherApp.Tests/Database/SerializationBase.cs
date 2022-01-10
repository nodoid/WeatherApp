namespace WeatherApps.Tests.Database
{
    public abstract class SerializationTestBase
    {
        /// <summary>
        /// Clean samples given in the spec document.
        /// </summary>
        /// <remarks>
        /// <para>The spec document samples require cleaning as the JSON does not
        /// match the defaults for the .NET serializer and is otherwise
        /// inconsistent.</para>
        /// <para>
        /// Note that there is an odd failure on the build machine where the string
        /// literal seems to include different line endings... Rather than figure it out
        /// I just strip them.
        /// </para>
        /// </remarks>
        /// <param name="sample">Sample text from the spec to clean.</param>
        /// <returns>Cleaned sample text.</returns>
        protected string Clean(string sample)
        {
            return sample
                .Replace("\r\n", string.Empty)
                .Replace("\n", string.Empty)
                .Replace(": ", ":");
        }
    }
}