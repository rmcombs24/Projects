using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassMediaEditor
{
    public static class Version
    {
        private static int major        = 1; //Reserved for major changes such as UI redesign
        private static int feature      = 2; //Reserved for features that might be added
        private static int development  = 3; //Reserved for backend updates/refactoring
        private static int hotfix       = 0; //Reserved for hotfixes

        private static string versionString = String.Format("v{0}.{1}.{2}.{3}", major, feature, development, hotfix);

        public static string GetVersionNumber()
        {
            return versionString;
        }
    }
}
