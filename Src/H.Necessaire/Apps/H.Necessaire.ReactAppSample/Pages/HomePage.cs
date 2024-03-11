﻿using Bridge.React;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp;
using H.Necessaire.ReactAppSample.Components;
using System;

namespace H.Necessaire.ReactAppSample.Pages
{
    public class HomePage : PageBase<HomePage.Props, HomePage.State>
    {
        public HomePage() : base(new Props(), null) { }

        public override ReactElement Render()
        {
            return
                new DefaultChrome(

                    new FormLayout(
                        new FormLayout.Props { 
                            LayoutMode = FormLayoutMode.OnePerRow,
                        }
                        ,
                        DOM.H1("Hello there !")

                        , new Button(new Button.Props
                        {

                            OnClick = async () =>
                            {
                                await Logger.LogDebug("VersionNumber Comparison Works", new VersionNumber(1, 1) == VersionNumber.Unknown);
                                await Alert("Test Alert");
                                await Confirm("Test confirm");
                            },

                        }, "Debug")
                        ,
                        new FontIcon(new FontIcon.Props { Provider = FontIcon.Provider.FontAwesome, IconName = "user" })
                        ,
                        new StringDataViewComponent(new DataViewComponentProps<string> { Data = "Hin", DataViewConfig = new DataViewConfig { Label = "First Name" } })
                        ,
                        new DefaultDataViewComponent<string>(new DataViewComponentProps<string> { Data = "Tee", DataViewConfig = new DataViewConfig { Label = "Last Name", Description = "asf sadf a sdf as df as df sa f sad f" } })
                        ,
                        DataViewComponentFactory.BuildViewerFor(17.1234123412m, x => x.Label = "Number with decimals")
                        ,
                        DataViewComponentFactory.BuildViewerFor(-17, x => { x.Label = "Integer"; x.Numeric.NumberOfDecimals = 0; })
                        ,
                        DataViewComponentFactory.BuildViewerFor(Guid.NewGuid(), x => { x.Label = "Guid via DefaultView"; })
                        ,
                        DataViewComponentFactory.BuildViewerFor(TimeSpan.FromDays(42.33), x => { x.Label = "Test dataView builder for timespan"; })
                        ,
                        DataViewComponentFactory.BuildViewerFor(DateTime.Now, x => { 
                            x.Label = "Test dataView builder for datetime";
                            x.Object.UseDefaultDataViewer = true;
                            x.Numeric.NumberOfDecimals = 0;
                        })
                        ,
                        new DataEditComponent(new DataEditComponent.Props { })
                    )

                );
        }


        public class State : PageStateBase { }

        public class Props : PagePropsBase { }
    }
}
