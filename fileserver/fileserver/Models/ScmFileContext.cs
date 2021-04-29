using Microsoft.EntityFrameworkCore;

namespace  FileServer.Models
{
    /// <summary>
    /// 文件数据对象
    /// </summary>
    public partial class FileContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// 构造
        /// </summary>
        /// <param name="options">参数</param>
        public FileContext(DbContextOptions<FileContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets 文件信息
        /// </summary>
        public virtual DbSet<DbFileInfo> FileInfo { get; set; }

        /// <summary>
        /// Gets or sets 文件操作历史
        /// </summary>
        public virtual DbSet<FileOperate> FileOperate { get; set; }

        /// <summary>
        /// 数据模型
        /// </summary>
        /// <param name="modelBuilder">模型建立器</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFileInfo>(entity =>
            {
                entity.ToTable("file_info");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasMaxLength(32)
                    .ValueGeneratedNever();

                entity.Property(e => e.DownloadNum)
                    .HasColumnName("DOWNLOAD_NUM")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("IS_DELETE")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.IsImage)
                    .HasColumnName("IS_IMAGE")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.IsTemp)
                    .HasColumnName("IS_TEMP")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Md5)
                    .HasColumnName("MD5")
                    .HasMaxLength(32);

                entity.Property(e => e.Mime)
                    .HasColumnName("MIME")
                    .HasMaxLength(100);

                entity.Property(e => e.OriginalName)
                    .HasColumnName("ORIGINAL_NAME")
                    .HasMaxLength(200);

                entity.Property(e => e.Path)
                    .HasColumnName("PATH")
                    .HasMaxLength(254);

                entity.Property(e => e.Sha256)
                    .HasColumnName("SHA256")
                    .HasMaxLength(256);

                entity.Property(e => e.Size)
                    .HasColumnName("SIZE")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.ThumbnailPath)
                    .HasColumnName("THUMBNAIL_PATH")
                    .HasMaxLength(256);

                entity.Property(e => e.Ttid)
                    .HasColumnName("TTID")
                    .HasMaxLength(150);

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("UPDATE_DATE")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<FileOperate>(entity =>
            {
                entity.ToTable("file_operate");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Action)
                    .HasColumnName("ACTION")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.FileId)
                    .HasColumnName("FILE_ID")
                    .HasMaxLength(32);

                entity.Property(e => e.OperateDate)
                    .HasColumnName("OPERATE_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.Operator)
                    .HasColumnName("OPERATOR")
                    .HasMaxLength(50);
            });
        }
    }
}
