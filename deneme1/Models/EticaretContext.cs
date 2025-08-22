using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace eticaret.Models;

public partial class EticaretContext : DbContext
{
    public EticaretContext()
    {
    }

    public EticaretContext(DbContextOptions<EticaretContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AltKategori> AltKategoris { get; set; }

    public virtual DbSet<AnaKategori> AnaKategoris { get; set; }

    public virtual DbSet<Kullanici> Kullanicis { get; set; }

    public virtual DbSet<KullaniciBilgileri> KullaniciBilgileris { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-GBH2I4C\\SQLEXPRESS;Database=Eticaret;User Id=sa;Password=1;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AltKategori>(entity =>
        {
            entity.ToTable("AltKategori");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnaKategoriId).HasColumnName("AnaKategori_id");
            entity.Property(e => e.KategoriAdi)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UstKategoriId).HasColumnName("UstKategori_id");
        });

        modelBuilder.Entity<AnaKategori>(entity =>
        {
            entity.ToTable("AnaKategori");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.KategoriAdi)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Kategori_Adi");
        });

        modelBuilder.Entity<Kullanici>(entity =>
        {
            entity.ToTable("Kullanici");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Durum).HasColumnName("durum");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<KullaniciBilgileri>(entity =>
        {
            entity.ToTable("Kullanici_bilgileri");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ad)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ad");
            entity.Property(e => e.KullaniciId).HasColumnName("kullanici_id");
            entity.Property(e => e.Mail)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("mail");
            entity.Property(e => e.Soyad)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("soyad");
            entity.Property(e => e.Telefon)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("telefon");
            entity.Property(e => e.Yetki).HasColumnName("yetki");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
