﻿using Chat.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Data.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");
            builder.Property(s => s.Content).IsRequired().HasMaxLength(500);

            builder.HasOne(s => s.ToRoom)
                .WithMany(m => m.Messages)
                .HasForeignKey(s => s.ToRoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
