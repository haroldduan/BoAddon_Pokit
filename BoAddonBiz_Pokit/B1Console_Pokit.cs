using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoAddonBiz_Pokit
{
    public class B1Console_Pokit : SAPBoAddon.B1BusinessConsole
    {
        public override bool CheckEnvironment()
        {
            return true;
        }

        protected override void RegisterForms()
        {
            foreach (Type curType in System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(c => c.BaseType == typeof(SAPBoAddon.B1Form)))
            {
                if (!curType.IsAbstract)
                {
                    Activator.CreateInstance(curType, true);
                }
                else
                {
                    foreach (Type curSubType in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
                    {
                        if (curSubType.BaseType == curType)
                        {
                            Activator.CreateInstance(curSubType, true);
                        }
                    }
                }
            }
        }

        protected override void RegisterMenus()
        {
            B1Menu_Pokit oMenu = new B1Menu_Pokit();
        }

        protected override void RegisterOthers()
        {

        }
    }
}
