using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MetaTag> MetaTags { get; set; }
    public virtual DbSet<MetricType> MetricTypes { get; set; }
    public virtual DbSet<ObservationDefinition> ObservationDefinitions { get; set; }
    public virtual DbSet<ObservationType> ObservationTypes { get; set; }
    public virtual DbSet<ScientificDiscipline> ScientificDisciplines { get; set; }
    public virtual DbSet<Subject> Subjects { get; set; }
    public virtual DbSet<SubjectRecord> SubjectRecords { get; set; }
    public virtual DbSet<SubjectType> SubjectTypes { get; set; }
    public virtual DbSet<Unit> Units { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasCharSet("utf8mb4");

        modelBuilder.Entity<MetaTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("MetaTag");
            entity.HasIndex(e => e.Name, "Name").IsUnique();
            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<MetricType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("MetricType");
            entity.HasIndex(e => e.Name, "Name").IsUnique();
            entity.HasIndex(e => e.UnitId, "FK_MetricType_Unit");
            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Name).HasColumnType("text");
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.UnitId).HasColumnType("int(11)");

            entity.HasOne(d => d.Unit).WithMany(p => p.MetricTypes)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MetricType_Unit");
        });

        modelBuilder.Entity<ObservationDefinition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("ObservationDefinition");
            entity.HasIndex(e => e.Name, "Name").IsUnique();
            entity.HasIndex(e => e.ObservationTypeId, "FK_ObservationDefinition_ObservationType");
            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.IsSingular).HasDefaultValueSql("'0'");
            entity.Property(e => e.MaximumValue).HasPrecision(10, 2);
            entity.Property(e => e.MinimumValue).HasPrecision(10, 2);
            entity.Property(e => e.ObservationTypeId).HasColumnType("int(11)");

            entity.HasOne(d => d.ObservationType).WithMany(p => p.ObservationDefinitions)
                .HasForeignKey(d => d.ObservationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ObservationDefinition_ObservationType");

            entity.HasMany(d => d.ScientificDisciplines).WithMany(p => p.ObservationDefinitions)
                .UsingEntity<Dictionary<string, object>>(
                    "ObservationDefinitionDiscipline",
                    r => r.HasOne<ScientificDiscipline>().WithMany()
                        .HasForeignKey("ScientificDisciplineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_ObservationDefinitionDiscipline_ScientificDiscipline"),
                    l => l.HasOne<ObservationDefinition>().WithMany()
                        .HasForeignKey("ObservationDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_ObservationDefinitionDiscipline_ObservationDefinition"),
                    j =>
                    {
                        j.HasKey("ObservationDefinitionId", "ScientificDisciplineId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("ObservationDefinitionDiscipline");
                        j.HasIndex(new[] { "ScientificDisciplineId" }, "FK_ObservationDefinitionDiscipline_ScientificDiscipline");
                        j.IndexerProperty<int>("ObservationDefinitionId").HasColumnType("int(11)");
                        j.IndexerProperty<int>("ScientificDisciplineId").HasColumnType("int(11)");
                    });

            entity.HasMany(d => d.MetricTypes).WithMany(p => p.ObservationDefinitions)
                .UsingEntity<Dictionary<string, object>>(
                    "ObservationDefinitionMetricType",
                    r => r.HasOne<MetricType>().WithMany()
                        .HasForeignKey("MetricTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_ObservationDefinitionMetricType_MetricType"),
                    l => l.HasOne<ObservationDefinition>().WithMany()
                        .HasForeignKey("ObservationDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_ObservationDefinitionMetricType_ObservationDefinition"),
                    j =>
                    {
                        j.HasKey("ObservationDefinitionId", "MetricTypeId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("ObservationDefinitionMetricType");
                        j.HasIndex(new[] { "MetricTypeId" }, "FK_ObservationDefinitionMetricType_MetricType");
                        j.IndexerProperty<int>("ObservationDefinitionId").HasColumnType("int(11)");
                        j.IndexerProperty<int>("MetricTypeId").HasColumnType("int(11)");
                    });

            entity.HasMany(d => d.Units).WithMany(p => p.ObservationDefinitions)
                .UsingEntity<Dictionary<string, object>>(
                    "ObservationDefinitionUnit",
                    r => r.HasOne<Unit>().WithMany()
                        .HasForeignKey("UnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_ObservationDefinitionUnit_Unit"),
                    l => l.HasOne<ObservationDefinition>().WithMany()
                        .HasForeignKey("ObservationDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_ObservationDefinitionUnit_ObservationDefinition"),
                    j =>
                    {
                        j.HasKey("ObservationDefinitionId", "UnitId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("ObservationDefinitionUnit");
                        j.HasIndex(new[] { "UnitId" }, "FK_ObservationDefinitionUnit_Unit");
                        j.IndexerProperty<int>("ObservationDefinitionId").HasColumnType("int(11)");
                        j.IndexerProperty<int>("UnitId").HasColumnType("int(11)");
                    });
        });

        modelBuilder.Entity<ObservationType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("ObservationType");
            entity.HasIndex(e => e.Name, "Name").IsUnique();
            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<ScientificDiscipline>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("ScientificDiscipline");
            entity.HasIndex(e => e.Name, "Name").IsUnique();
            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("Subject");
            entity.HasIndex(e => e.SubjectTypeId, "FK_Subject_SubjectType");
            entity.HasIndex(e => e.Name, "IDX_Subject_Name");
            entity.HasIndex(e => new { e.Name, e.ArrivalDate }, "Name").IsUnique();
            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Notes).HasColumnType("text");
            entity.Property(e => e.Sex).HasColumnType("tinyint(4)");
            entity.Property(e => e.SubjectTypeId).HasColumnType("int(11)");
            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.Property(e => e.CreatedBy).HasMaxLength(450);
            entity.Property(e => e.UpdatedBy).HasMaxLength(450);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Permissions).HasColumnType("int(11)");

            entity.HasOne(d => d.SubjectType).WithMany(p => p.Subjects)
                .HasForeignKey(d => d.SubjectTypeId)
                .HasConstraintName("FK_Subject_SubjectType");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Subject_AspNetUsers_UserId");

            entity.HasMany(d => d.MetaTags).WithMany(p => p.Subjects)
                .UsingEntity<Dictionary<string, object>>(
                    "SubjectMetaTag",
                    r => r.HasOne<MetaTag>().WithMany()
                        .HasForeignKey("MetaTagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_SubjectMetaTag_MetaTag"),
                    l => l.HasOne<Subject>().WithMany()
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_SubjectMetaTag_Subject"),
                    j =>
                    {
                        j.HasKey("SubjectId", "MetaTagId").HasName("PRIMARY").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("SubjectMetaTag");
                        j.HasIndex(new[] { "MetaTagId" }, "FK_SubjectMetaTag_MetaTag");
                        j.IndexerProperty<int>("SubjectId").HasColumnType("int(11)");
                        j.IndexerProperty<int>("MetaTagId").HasColumnType("int(11)");
                    });
        });

        modelBuilder.Entity<SubjectRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("SubjectRecord");
            entity.HasIndex(e => e.MetricTypeId, "IDX_SubjectRecord_MetricTypeId");
            entity.HasIndex(e => e.ObservationDefinitionId, "IDX_SubjectRecord_ObservationDefinitionId");
            entity.HasIndex(e => e.RecordTime, "IDX_SubjectRecord_RecordTime");
            entity.HasIndex(e => e.SubjectId, "IDX_SubjectRecord_SubjectId");
            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CreatedBy).HasMaxLength(450);
            entity.Property(e => e.UpdatedBy).HasMaxLength(450);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.MetricTypeId).HasColumnType("int(11)");
            entity.Property(e => e.MetricValue).HasPrecision(10, 2);
            entity.Property(e => e.Note).HasColumnType("text");
            entity.Property(e => e.ObservationDefinitionId).HasColumnType("int(11)");
            entity.Property(e => e.RecordTime).HasDefaultValueSql("current_timestamp()").HasColumnType("datetime");
            entity.Property(e => e.SubjectId).HasColumnType("int(11)");

            entity.HasOne(d => d.MetricType).WithMany(p => p.SubjectRecords)
                .HasForeignKey(d => d.MetricTypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_SubjectRecord_MetricType");

            entity.HasOne(d => d.ObservationDefinition).WithMany(p => p.SubjectRecords)
                .HasForeignKey(d => d.ObservationDefinitionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SubjectRecord_ObservationDefinition");

            entity.HasOne(d => d.Subject).WithMany(p => p.SubjectRecords)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK_SubjectRecord_Subject");

            entity.HasMany(d => d.MetaTags).WithMany(p => p.SubjectRecords)
                .UsingEntity<Dictionary<string, object>>(
                    "SubjectRecordMetaTag",
                    r => r.HasOne<MetaTag>().WithMany()
                        .HasForeignKey("MetaTagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_SubjectRecordMetaTag_MetaTag"),
                    l => l.HasOne<SubjectRecord>().WithMany()
                        .HasForeignKey("SubjectRecordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_SubjectRecordMetaTag_SubjectRecord"),
                    j =>
                    {
                        j.HasKey("SubjectRecordId", "MetaTagId").HasName("PRIMARY").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("SubjectRecordMetaTag");
                        j.HasIndex(new[] { "MetaTagId" }, "FK_SubjectRecordMetaTag_MetaTag");
                        j.IndexerProperty<int>("SubjectRecordId").HasColumnType("int(11)");
                        j.IndexerProperty<int>("MetaTagId").HasColumnType("int(11)");
                    });
        });

        modelBuilder.Entity<SubjectType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("SubjectType");
            entity.HasIndex(e => e.Name, "Name").IsUnique();
            entity.HasIndex(e => new { e.Name, e.ScientificName, e.Clade }, "IX_SubjectType_Name_ScientificName_Clade").IsUnique();
            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.ScientificName).HasColumnType("text");
            entity.Property(e => e.Clade).HasMaxLength(100).HasColumnType("varchar(100)");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("Unit");
            entity.HasIndex(e => e.Name, "Name").IsUnique();
            entity.HasIndex(e => e.UnitSymbol, "UnitSymbol").IsUnique();
            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.Name).HasMaxLength(20);
            entity.Property(e => e.UnitSymbol).HasMaxLength(5);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}