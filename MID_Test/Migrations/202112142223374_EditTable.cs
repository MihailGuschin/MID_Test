namespace MID_Test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "Gender", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Employees", "Gender", c => c.Int(nullable: false));
        }
    }
}
