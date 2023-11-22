using System.Web;
using System.Web.Optimization;

namespace DConfig
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //System Scripts
            bundles.Add(new ScriptBundle("~/bundles/libs").Include(
                        "~/Scripts/libs/jquery/jquery-1.11.2.js",
                        "~/Scripts/libs/jquery/jquery-migrate-1.2.1.js",
                        "~/Scripts/libs/jquery-ui/jquery-ui.min.js",
                        "~/Scripts/libs/bootstrap/bootstrap.js",
                        "~/Scripts/libs/spin.js/spin.js",
                        "~/Scripts/libs/autosize/jquery.autosize.js",
                        "~/Scripts/libs/select2/select2.js",
                        "~/Scripts/libs/bootstrap-tagsinput/bootstrap-tagsinput.js",
                        "~/Scripts/libs/multi-select/jquery.multi-select.js",
                        "~/Scripts/libs/inputmask/jquery.inputmask.bundle.js",
                        "~/Scripts/libs/moment/moment.js",
                        "~/Scripts/libs/bootstrap-datepicker/bootstrap-datepicker.js",
                        "~/Scripts/libs/bootstrap-colorpicker/bootstrap-colorpicker.js",
                        "~/Scripts/libs/dropzone/dropzone.js",
                        "~/Scripts/libs/typeahead/typeahead.bundle.js",
                        "~/Scripts/libs/nanoscroller/jquery.nanoscroller.js",
                        "~/Scripts/libs/toastr/toastr.js",
                        "~/Scripts/libs/context-menu/jquery.contextMenu.js",
                        "~/Scripts/libs/wizard/jquery.bootstrap.wizard.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/system").Include(
            "~/Scripts/core/source/App.js",
            "~/Scripts/core/source/AppNavigation.js",
            "~/Scripts/core/source/AppOffcanvas.js",
            "~/Scripts/core/source/AppCard.js",
            "~/Scripts/core/source/AppForm.js",
            "~/Scripts/core/source/AppNavSearch.js",
            "~/Scripts/core/source/AppVendor.js",
            "~/Scripts/core/demo/Demo.js",
            "~/Scripts/core/demo/DemoFormComponents.js"
            ));

            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/libs/jquery/jquery-1.11.2.min.js"));
                        //"~/Scripts/jquery.placeholder.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.min.js",
            //          "~/Scripts/respond.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/angularJS").Include(
                      "~/Scripts/angular.js",
                      "~/Scripts/angular-route.js",
                      "~/Scripts/angular-sanitize.js",
                      "~/Scripts/angular-cookies.js",
                      "~/Scripts/ng-grid.js",
                      "~/Scripts/ng-csv.js",
                      "~/Scripts/ui-bootstrap-tpls-0.12.0.js",
                      "~/Scripts/libs/bootstrap-tagsinput/bootstrap-tagsinput-angular.js",
                      //"~/Scripts/d3.js",
                      //"~/Scripts/nv.d3.js",
                      //"~/Scripts/angularjs-nvd3-directives.js",
                      "~/Scripts/angular-file-upload-shim.js",
                      "~/Scripts/angular-file-upload.min.js",
                      "~/Scripts/angular-ui/sortable.js",
                      "~/Scripts/ckeditor/ckeditor.js",
                      "~/Scripts/smart-table.min.js",
                      "~/Scripts/clipboard.js"));

            bundles.Add(new ScriptBundle("~/bundles/generalForms").Include(
                    "~/Scripts/forms/jquery-1.7.1.js",
                    "~/Scripts/forms/jquery.mobile.custom.js",
                    "~/Scripts/forms/jquery.unobtrusive-ajax.js",
                    "~/Scripts/forms/jquery.easing.1.3.js",
                    "~/Scripts/forms/jquery.animate-enhanced.js",
                    "~/Scripts/forms/bootstrap.js",
                    "~/Scripts/forms/respond.js",
                    "~/Scripts/forms/jquery.validate.js",
                    "~/Scripts/forms/jquery-ui-1.8.20.js",
                    "~/Scripts/forms/jquery.placeholder.js",
                    "~/Scripts/forms/startup.js"));


            //bundles.Add(new ScriptBundle("~/bundles/dconfig.shared").IncludeDirectory("~/Core/Shared","*.js",false));
            bundles.Add(new ScriptBundle("~/bundles/dconfig_shared").Include(
                "~/Core/Shared/app.js",
                "~/Core/Shared/appUserMessagesProvider.js",
                "~/Core/Shared/eventsProvider.js",
                "~/Core/Shared/directives.js",
                "~/Core/Shared/breadCrumpsProvider.js",
                "~/Core/Shared/authHttpInterceptor.js",
                "~/Core/Shared/userPermissionsProvider.js",
                "~/Core/Shared/globalVariablesProvider.js",
                "~/Core/Shared/filters.js",
                "~/Core/Shared/viewsProvider.js",
                "~/Core/Shared/scopeService.js",
                "~/Core/Shared/utilities.js",
                "~/Core/Shared/intentsProvider.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/dconfig_account").IncludeDirectory("~/Core/Account", "*.js", false));
            bundles.Add(new ScriptBundle("~/bundles/dconfig_account_controllers").IncludeDirectory("~/Core/Account/Controllers", "*.js", false));
            bundles.Add(new ScriptBundle("~/bundles/dconfig_explorer").IncludeDirectory("~/Core/Explorer", "*.js", false));
            bundles.Add(new ScriptBundle("~/bundles/dconfig_explorer_controllers").IncludeDirectory("~/Core/Explorer/Controllers", "*.js", false));

            bundles.Add(new ScriptBundle("~/bundles/dconfig_controls").Include(
                       "~/Scripts/jstree.js",
                       //"~/Scripts/jquery.ptTimeSelect.js",
                       //"~/Scripts/nicEdit.min.js",
                       "~/Scripts/ace/ace.js"
                       //"~/Scripts/libs/jquery-ui/jquery-ui.min.js"
                       //"~/Scripts/jquery-ui.js"
                       ));

            
            //System Styles
            bundles.Add(new StyleBundle("~/Content/styleslibs").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/materialadmin.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/material-design-iconic-font.min.css",
                      "~/Content/libs/select2/select2.css",
                      "~/Content/libs/multi-select/multi-select.css",
                      "~/Content/libs/bootstrap-datepicker/datepicker3.css",
                      "~/Content/libs/jquery-ui/jquery-ui-theme.css",
                      "~/Content/libs/bootstrap-colorpicker/bootstrap-colorpicker.css",
                      "~/Content/libs/bootstrap-tagsinput/bootstrap-tagsinput.css",
                      "~/Content/libs/typeahead/typeahead.css",
                      "~/Content/libs/dropzone/dropzone-theme.css",
                      "~/Content/libs/toastr/toastr.css",
                      "~/Content/ng-grid.css",
                      "~/Content/libs/context-menu/jquery.contextMenu.css",
                      "~/Content/libs/wizard/wizard.css"
                      ));
                      //"~/Content/bootstrap-theme.min.css",
                      //"~/Content/ng-grid.min.css",
                      //"~/Content/jquery-ui.css",
                      //"~/Content/nv.d3.css"));

            bundles.Add(new StyleBundle("~/Content/dconfig").Include(
                      "~/Content/DConfigOS/Layout.css"));

            bundles.Add(new StyleBundle("~/Content/dconfig_account").Include(
                      "~/Content/DConfigOS/Account/Account.css"));

            bundles.Add(new StyleBundle("~/Content/dconfig_explorer").Include(
                      "~/Content/DConfigOS/Explorer/Explorer.css",
                      "~/Content/DConfigOS/Explorer/AppsLayouts.css",
                      "~/Content/DConfigOS/Explorer/jquery.ferro.ferroMenu.css",
                      "~/Content/DConfigOS/Explorer/font-awesome-4.2.0/css/font-awesome.min.css",
                      "~/Content/DConfigOS/Explorer/jstree/themes/default/style.min.css",
                      "~/Content/DConfigOS/Explorer/jQuery.ptTimeSelect/jquery.ptTimeSelect"));

        }
    }
}
