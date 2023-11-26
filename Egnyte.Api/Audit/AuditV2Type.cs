namespace Egnyte.Api.Audit
{
    public enum AuditV2Type
    {
        /// <summary>
        /// File Audit
        /// </summary>
        FILE_AUDIT,

        /// <summary>
        /// Login Audit
        /// </summary>
        LOGIN_AUDIT,

        /// <summary>
        /// Permissions Audit
        /// </summary>
        PERMISSION_AUDIT,

        /// <summary>
        /// User Audit
        /// </summary>
        USER_AUDIT,

        /// <summary>
        /// Group Audit
        /// </summary>
        GROUP_AUDIT,

        /// <summary>
        /// WG Settings Audit
        /// </summary>
        WG_SETTINGS_AUDIT,

        /// <summary>
        /// Workflow Audit
        /// </summary>
        WORKFLOW_AUDIT,

        /// <summary>
        /// Quality Docs Audit
        /// </summary>
        QUALITY_DOCS_AUDIT,

        /// <summary>
        /// Quality Docs Categories Audit
        /// </summary>
        QUALITY_DOCS_CATEGORIES_AUDIT,

        /// <summary>
        /// Any
        /// </summary>
        ANY
    }
}
