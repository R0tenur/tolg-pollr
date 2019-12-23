namespace TolgPollr.Worker.Configuration
{
    public static class TerminalCommandConstants
    {
        public const string LircCommand = "irsend";

        public static readonly string[] LircHeatParameters = new[] { "send_once", "ac", "on" };
        public static readonly string[] LircCoolParameters = new[] { "send_once", "ac", "on" };
    }
}