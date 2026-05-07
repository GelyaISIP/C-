namespace LogisticSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveEmailAndPhoneFromClients : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Clients", "Email");
            DropColumn("dbo.Clients", "Phone");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Clients", "Phone", c => c.String());
            AddColumn("dbo.Clients", "Email", c => c.String());
        }
    }
}
