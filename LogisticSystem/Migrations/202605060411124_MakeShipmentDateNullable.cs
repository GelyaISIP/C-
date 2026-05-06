namespace LogisticSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeShipmentDateNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Shipments", "ShipmentDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Shipments", "ShipmentDate", c => c.DateTime(nullable: false));
        }
    }
}
