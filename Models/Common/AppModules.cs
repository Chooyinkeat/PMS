namespace PMS.Models.Common
{
    public static class AppModules
    {
        public const string User = "User";
        public const string UserGroup = "UserGroup";
        public const string Customer = "Customer";
        public const string PersonInCharge = "PersonInCharge";
        public const string Location = "Location";
        public const string Project = "Project";
        public const string Payment = "Payment";
        public const string Document = "Document";

        public static readonly List<ModuleDefinition> AllModules = new()
        {
            new ModuleDefinition(User, "User Maintenance", "Manage system user accounts, login credentials, and user statuses.", "users"),
            new ModuleDefinition(UserGroup, "User Group & Permissions", "Manage user groups and access control permissions (View, Add, Edit, Delete, Print).", "shield-check"),
            new ModuleDefinition(Customer, "Customer Maintenance", "Manage client/customer profiles and relationships.", "building"),
            new ModuleDefinition(PersonInCharge, "Person In Charge (PIC)", "Manage customer contact persons and representatives.", "user-check"),
            new ModuleDefinition(Location, "Location Maintenance", "Manage work sites, physical locations, and branch offices.", "map-pin"),
            new ModuleDefinition(Project, "Project Management", "Manage projects, schedules, budgets, and project team members.", "briefcase"),
            new ModuleDefinition(Payment, "Payment Management", "Track project invoices, receipts, and payment statuses.", "wallet"),
            new ModuleDefinition(Document, "Document & Reports", "Generate, preview, and export official project management reports in PDF format.", "file-text")
        };
    }

    public record ModuleDefinition(string Code, string Name, string Description, string Icon);
}
