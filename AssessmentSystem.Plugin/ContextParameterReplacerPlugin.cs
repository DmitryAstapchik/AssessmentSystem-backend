//using System;
//using Microsoft.VisualStudio.TestTools.WebTesting;

//namespace AssessmentSystem.Plugin
//{
//    public class ContextParameterReplacerPlugin : WebTestRequestPlugin
//    {
//        Random _rnd = new Random();

//        public string ParametersName { get; set; }

//        public string ParameterTemplate { get; set; }

//        public override void PreRequestDataBinding(object sender, PreRequestDataBindingEventArgs e)
//        {
//            string value = ParameterTemplate.Replace("{r}", _rnd.Next().ToString());

//            foreach (var name in ParametersName.Split(','))
//            {
//                e.WebTest.Context.Add(name, value);
//            }

//            base.PreRequestDataBinding(sender, e);
//        }
//    }

//    public class EmailContextParameterReplacerPlugin : WebTestRequestPlugin
//    {
//        Random _rnd = new Random();

//        public string ParametersName { get; set; }

//        public string ParameterTemplate { get; set; }

//        public override void PreRequestDataBinding(object sender, PreRequestDataBindingEventArgs e)
//        {
//            string value = ParameterTemplate.Replace("{r}", _rnd.Next().ToString());

//            foreach (var name in ParametersName.Split(','))
//            {
//                e.WebTest.Context.Add(name, value);
//            }

//            base.PreRequestDataBinding(sender, e);
//        }
//    }
//}