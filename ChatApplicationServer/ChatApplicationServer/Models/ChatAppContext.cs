using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ChatApplicationServer.Models;

public partial class ChatAppContext : DbContext
{
    public ChatAppContext()
    {
    }

    public ChatAppContext(DbContextOptions<ChatAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChatRoom> ChatRooms { get; set; }

    public virtual DbSet<Connection> Connections { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MessagesChatRoom> MessagesChatRooms { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersChatRoom> UsersChatRooms { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-2Q397LJ\\SQLEXPRESS;Database=ChatApp;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatRoom__3214EC0728F76686");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Connection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Connecti__3214EC07D9F91D48");

            entity.ToTable("Connection");

            entity.Property(e => e.SignalRid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SignalRId");
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Connections)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Connectio__UserI__2C3393D0");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Messages__3214EC07DEAC83F5");

            entity.Property(e => e.SentAt).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.Chat).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__ChatId__29572725");

            entity.HasOne(d => d.User).WithMany(p => p.Messages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__UserId__286302EC");
        });

        modelBuilder.Entity<MessagesChatRoom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Messages__3214EC079D2C9E48");

            entity.HasOne(d => d.ChatRoom).WithMany(p => p.MessagesChatRooms)
                .HasForeignKey(d => d.ChatRoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MessagesC__ChatR__2F10007B");

            entity.HasOne(d => d.Message).WithMany(p => p.MessagesChatRooms)
                .HasForeignKey(d => d.MessageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MessagesC__Messa__2E1BDC42");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07CC9C33D8");

            entity.Property(e => e.TokenCreated).HasColumnType("datetime");
            entity.Property(e => e.TokenExpires).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<UsersChatRoom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UsersCha__3214EC077835776D");

            entity.HasOne(d => d.ChatRoom).WithMany(p => p.UsersChatRooms)
                .HasForeignKey(d => d.ChatRoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UsersChat__ChatR__31EC6D26");

            entity.HasOne(d => d.User).WithMany(p => p.UsersChatRooms)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UsersChat__UserI__30F848ED");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
