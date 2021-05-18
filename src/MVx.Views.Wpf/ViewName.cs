namespace MVx.Views
{
    public static class ViewName
    {
        public static string For(string model)
        {
            return model.Replace("Model", string.Empty);
        }
    }
}
