namespace Cake.SqlServer
{
    /// <summary>
    /// This enum specifies what user mode the database should be in.
    /// </summary>
    public enum DbUserMode
    {
        /// <summary>
        /// This is the default database user access mode. In this database user access mode any user who have permission to access the database can access the database.
        /// </summary>
        MultiUser,

        /// <summary>
        /// In this user mode at any given point of time only one user can access the database. The user can be any user who has access to the database.
        /// </summary>
        SingleUser,

        /// <summary>
        /// In this user mode only the users who have db_owner or db_creator permission can access.
        /// </summary>
        RestrictedUser,
    }
}
