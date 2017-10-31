using System.Collections.Specialized;
using System.Text;
using GutenTag.Hspi;
using Hspi.HspiPlugin3;
using Scheduler;

namespace HSPIPluginC.Sample
{
    public class SamplePage : Page
    {
        public override string GetLinkText()
        {
            return "monster";
        }

        public override string GetPageTitle()
        {
            return "waka";
        }

        public override bool RegisterInInterfaceConfigPage()
        {
            return true;
        }

        public override bool RegisterInInterfacesMenu()
        {
            return true;
        }

        protected override StringBuilder BuildPage(StringBuilder s,
            string user,
            int userRights,
            NameValueCollection queryString)
        {
            AddHeader(Root.HS.GetPageHeader(GetPageTitle(), GetPageTitle(), string.Empty, string.Empty, false, false));
            var form = new Form
            {
                new Table(TableClass.FullWidth)
                {
                    new TBody
                    {
                        new TR
                        {
                            new TD(TdClass.TableHeader, "Super awesome table")
                            {
                                { "colspan", "2" }
                            }
                        },
                        new TR
                        {
                            new TD(TdClass.TableColumn, "Test colum header"),
                            new TD(TdClass.TableColumn)
                            {
                                "Another column"
                            }
                        },
                        new TR
                        {
                            new TD(TdClass.TableCell, "Something"),
                            new TD(TdClass.TableCell)
                            {
                                new Div("Gets updated", "someid")
                                {
                                    
                                }
                            }
                        }
                    }
                }
            };

            s.Append(form);
            AddFooter(Root.HS.GetPageFooter());
            return s;
        }
    }
}