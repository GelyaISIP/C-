namespace LogisticSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlannedShipmentDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shipments", "PlannedShipmentDate", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shipments", "PlannedShipmentDate");
        }
    }
}
