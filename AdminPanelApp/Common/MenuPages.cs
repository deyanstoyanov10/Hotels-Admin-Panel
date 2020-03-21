namespace AdminPanelApp.Common
{
    using System;

    using Microsoft.AspNetCore.Mvc.Rendering;
    
    public static class MenuPages
    {
        public static string Dashboard => "Dashboard";

        public static string Purchase => "Purchase";

        public static string AddProducts => "AddProducts";

        public static string RegisterManager => "RegisterManager";

        public static string AddSuppliers => "AddSuppliers";

        public static string History => "History";

        public static string SupplierMessages => "SupplierMessages";

        public static string ChangeEmailTime => "ChangeEmailTime";

        public static string ChangeProductSupplier => "ChangeProductSupplier";

        public static string DashboardClass(ViewContext viewContext) => PageNavClass(viewContext, Dashboard);

        public static string PurchaseClass(ViewContext viewContext) => PageNavClass(viewContext, Purchase);

        public static string AddProductsClass(ViewContext viewContext) => PageNavClass(viewContext, AddProducts);

        public static string RegisterManagerClass(ViewContext viewContext) => PageNavClass(viewContext, RegisterManager);

        public static string AddSuppliersClass(ViewContext viewContext) => PageNavClass(viewContext, AddSuppliers);

        public static string HistoryClass(ViewContext viewContext) => PageNavClass(viewContext, History);

        public static string SupplierMessagesClass(ViewContext viewContext) => PageNavClass(viewContext, SupplierMessages);

        public static string ChangeEmailTimeClass(ViewContext viewContext) => PageNavClass(viewContext, ChangeEmailTime);

        public static string ChangeProductSupplierClass(ViewContext viewContext) => PageNavClass(viewContext, ChangeProductSupplier);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
