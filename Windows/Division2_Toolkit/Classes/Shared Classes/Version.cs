using System;

public static class Version
{
    private static int ReleaseNumber = 0; //Current Release. Updated for each release, UI change
    private static int FeatureNumber = 7; //Current Feature. Updated for each feature in a release
    private static int DevelopmentNumber = 10; //Development. Updated for any updates to dev  that arent features (refactors, classes, etc)
    private static int HotfixNumber = 0; //Hotfix. For any updates because of hotfixes

    static public string GetVersion()
    {
        return String.Format("{0}.{1}.{2}.{3}", ReleaseNumber, FeatureNumber, DevelopmentNumber, HotfixNumber);

    }
}
