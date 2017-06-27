namespace Domain.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ControleAtividadesEntities : DbContext
    {
        public ControleAtividadesEntities()
            : base("name=ControleAtividadesEntities")
        { }

        public virtual DbSet<ca_atividade> ca_atividade { get; set; }
        public virtual DbSet<ca_atividadesPublicadas> ca_atividadesPublicadas { get; set; }
        public virtual DbSet<ca_publicacao> ca_publicacao { get; set; }
        public virtual DbSet<ca_usuario> ca_usuario { get; set; }
        public virtual DbSet<tb_usuario> tb_usuario { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ca_atividade>()
                .Property(e => e.sistema)
                .IsUnicode(false);

            modelBuilder.Entity<ca_atividade>()
                .HasMany(e => e.ca_atividadesPublicadas)
                .WithRequired(e => e.ca_atividade)
                .HasForeignKey(e => e.id_atividade)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ca_publicacao>()
                .Property(e => e.sistema)
                .IsUnicode(false);

            modelBuilder.Entity<ca_publicacao>()
                .HasMany(e => e.ca_atividadesPublicadas)
                .WithRequired(e => e.ca_publicacao)
                .HasForeignKey(e => e.id_publicacao)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ca_usuario>()
                .Property(e => e.nome)
                .IsUnicode(false);

            modelBuilder.Entity<ca_usuario>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<ca_usuario>()
                .HasMany(e => e.ca_atividade)
                .WithOptional(e => e.ca_usuario)
                .HasForeignKey(e => e.id_usuario);

            modelBuilder.Entity<ca_usuario>()
                .HasMany(e => e.ca_publicacao)
                .WithRequired(e => e.ca_usuario)
                .HasForeignKey(e => e.id_usuario)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tb_usuario>()
                .Property(e => e.nome)
                .IsUnicode(false);

            modelBuilder.Entity<tb_usuario>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<tb_usuario>()
                .Property(e => e.senha)
                .IsUnicode(false);
        }
    }
}
