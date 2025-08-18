using System;
using System.Threading.Tasks;
using Reown.AppKit.Unity;
using Reown.Core.Common.Logging;
using Reown.Sign.Unity;
using UnityEngine;

public class TestWallet : MonoBehaviour
{
    private AppKitConfig Config { get; set; }

    private void Awake()
    {
        Config = new AppKitConfig
        {
            projectId = "690693bac3b1acc68e124450550fd69f",
            metadata = new Metadata(
                "EnkiGo",
                "EnkiGo Web App",
                "https://reown.com",
                "https://raw.githubusercontent.com/reown-com/reown-dotnet/main/media/appkit-icon.png",
                new RedirectData
                {
                    // Used by native wallets to redirect back to the app after approving requests
                    Native = "appkit-sample-unity://"
                }
            ),
            enableEmail = true,
            supportedChains = new[]
            {
                ChainConstants.Chains.Ethereum,
                ChainConstants.Chains.Optimism,
                ChainConstants.Chains.Arbitrum,
                ChainConstants.Chains.Ronin,
                ChainConstants.Chains.Avalanche,
                ChainConstants.Chains.Base,
                ChainConstants.Chains.Polygon,
            },
            socials = new[]
            {
                SocialLogin.Google,
                SocialLogin.X,
                SocialLogin.Apple,
            }
        };
    }

    private async void Start()
    {
        ReownLogger.Instance = new UnityLogger();

        await AppKit.InitializeAsync(Config);
        
    }

    public void Connect()
    {
        _ = ResumeSession();
    }
    
    public async Task ResumeSession()
    {
        // Try to resume account connection from the last session
        var resumed = await AppKit.ConnectorController.TryResumeSessionAsync();

        if (resumed)
        {
            // Continue to the game
            MyAccountConnectedHandler();
        }
        else
        {
            // Connect account
            AppKit.AccountConnected += (_, e) => MyAccountConnectedHandler();
            AppKit.OpenModal();
        }
    }

    private void MyAccountConnectedHandler()
    {
        
    }
}
