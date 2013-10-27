using aSkyImage.Resources;

namespace aSkyImage.Localization
{
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {

        }

        private static AppResources localizedResources = new AppResources();

        public AppResources AppResources
        {
            get { return localizedResources; }
        }
    }
}
