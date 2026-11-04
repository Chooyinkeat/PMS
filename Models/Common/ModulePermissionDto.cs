namespace PMS.Models.Common
{
    public class ModulePermissionDto
    {
        public string Module { get; set; } = string.Empty;
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanPrint { get; set; }

        public static ModulePermissionDto FullAccess(string module) => new()
        {
            Module = module,
            CanView = true,
            CanAdd = true,
            CanEdit = true,
            CanDelete = true,
            CanPrint = true
        };

        public static ModulePermissionDto NoAccess(string module) => new()
        {
            Module = module,
            CanView = false,
            CanAdd = false,
            CanEdit = false,
            CanDelete = false,
            CanPrint = false
        };
    }
}
