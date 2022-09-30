namespace AvoskaIsReal.Service
{
    public static class Extensions
    {
        public static string CutController(this string str)
        {
            return str.Replace("Controller", "");
        }

        public static string PathToUrl(this string path)
            => path.Replace('\\', '/');
    }
}
