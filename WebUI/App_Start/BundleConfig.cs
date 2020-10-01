using System.Web.Optimization;

namespace ERP.WebUI
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //================================================  js   =====================================================
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js",
                         "~/Scripts/umd/popper.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/custom-datetime-validate.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapjs").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/moment.min.js",
                        "~/Scripts/bootstrap-datepicker.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/metisMenujs").Include(
                      "~/Scripts/metisMenu.min.js",
                      "~/Scripts/raphael.min.js",
                      "~/Scripts/morris.min.js",
                      "~/Scripts/morris-data.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/dataTablesjs").Include(
                    "~/Scripts/DataTables/media/js/jquery.dataTables.min.js",
                    "~/Scripts/DataTables/media/js/dataTables.bootstrap4.min.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/dataTablesresponsivejs").Include(
                   "~/Scripts/DataTables/extensions/Responsive/js/dataTables.responsive.js",
                   "~/Scripts/DataTables/extensions/Responsive/js/responsive.bootstrap.js"
                   ));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/sbAdminjs").Include(
                        "~/Scripts/sb-admin-2.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/customjs").Include(
                        "~/Scripts/Select2/select2.min.js",
                        "~/Scripts/custom.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/tabjs").Include(
                "~/Scripts/polyfill.js",
                "~/Scripts/event-handler.js",
                "~/Scripts/selector-engine.js",
                "~/Scripts/data.js",
                "~/Scripts/manipulator.js",
                "~/Scripts/tab.js"
            ));

            //================================================   css   ===================================================
            bundles.Add(new StyleBundle("~/Content/bootstrapcss").Include(
                        "~/Content/jquery-ui.css",
                        "~/Content/bootstrap.css",
                        "~/Content/Select2/select2.min.css",
                        "~/Content/Select2/select2-bootstrap4.css",
                        "~/Content/bootstrap-datepicker.min.css"
                    ));

            bundles.Add(new StyleBundle("~/Content/metisMenucss").Include(
                       "~/Content/metisMenu.min.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/dataTablescss").Include(
                     "~/Content/DataTables/media/css/dataTables.bootstrap4.css"
                     ));

            bundles.Add(new StyleBundle("~/Content/dataTablesresponsivecss").Include(
                      "~/Content/DataTables/extensions/Responsive/css/responsive.bootstrap4.css",
                      "~/Content/DataTables/extensions/Responsive/css/responsive.bootstrap.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/sbAdmincss").Include(
                        "~/Content/sb-admin-2.css",
                        "~/Content/morris.css",
                         "~/Content/custom.css"
                       ));

            bundles.Add(new StyleBundle("~/Content/fontawesomecss").Include(
                        "~/Content/font-awesome.css"
                       ));
#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
