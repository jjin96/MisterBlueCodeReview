    [Table("Attachments")]
    public class Attachment
    {
        //외부 테이블의 AttachmentId 와 매핑됨.(파일보기,다운로드,삭제등에는 Id를 사용)
        [Key]
        public int Id { get; set; }

        //외부 테이블의 AttachmentIdentity 와 매핑됨.(외부 테이블에 특정글에 업로드된 파일들 전체 목록을 조회하고자 할때 사용 및 전체 다운로드시 사용)
        [Display(Name = "고유키")] //Identity를 이용한 참조를 이용하며 Id를 이용한 참조는 없게 만든다.. 테스트서버, 실서버 생성되는 id값이 다를 수 있기 때문에..
        [StringLength(40, ErrorMessage = "{0}에 유효한 값이 아닙니다.")]
        [Required]
        public string Identity { get; set; }

        [Display(Name = "참조테이블")] //어떤 테이블 데이터에 대한 업로드파일인지
        [StringLength(25, ErrorMessage = "{0}에 유효한 값이 아닙니다.")]
        [Required]
        public string ModelName { get; set; }

        [Display(Name = "파일구분")] //모델내에서 업로드된 파일의 구분을 위한 구분자(예 : 프로필사진인지, 대표이미지 인지..구분용)
        [StringLength(25, ErrorMessage = "{0}에 유효한 값이 아닙니다.")]
        public string Kind { get; set; }

        [Display(Name = "대표파일")]
        public bool IsDefault { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "생성일")]
        public DateTime CreatedDT { get; set; }

        [Required]
        [StringLength(80, ErrorMessage = "{0}에 유효한 값이 아닙니다.")]
        [Display(Name = "MIME")]
        public string FileType { get; set; }

        [Required]
        [Display(Name = "크기")]
        public int FileSize { get; set; }


        [Required]
        [Display(Name = "확장자")]
        [StringLength(5, ErrorMessage = "{0}에 유효한 값이 아닙니다.")]
        public string FileExtention { get; set; } //예 : png


        [Required]
        [Display(Name = "원본파일명")]
        [StringLength(56, ErrorMessage = "{0}에 유효한 값이 아닙니다.")]
        public string OriginalFile { get; set; }

        [Display(Name = "변환파일명")]
        [StringLength(70, ErrorMessage = "{0}에 유효한 값이 아닙니다.")]
        public string ReNameFile { get; set; } //this.OriginalFile 이 중복된 경우 서버에 저장할때는 리네임해서 저장한다.

        [Display(Name = "썸네일파일명")]
        [StringLength(75, ErrorMessage = "{0}에 유효한 값이 아닙니다.")]
        public string ThumbnailFile { get; set; }




        [Required]
        [Display(Name = "물리적경로")]
        [StringLength(200, ErrorMessage = "{0}에 유효한 값이 아닙니다.")]
        public string FilePath { get; set; } //예 : P:\Temporarily\upload\Data\20200505, Thumbnail등.. 해당경로에 자동으로 생성되는 파일이 존재할 수 있으므로 이 속성을 이용함

        [Required]
        [Display(Name = "물리적전체경로")]
        [StringLength(200, ErrorMessage = "{0}에 유효한 값이 아닙니다.")]
        public string FileFullPath { get; set; } //예 : P:\Temporarily\upload\Data\20200505\this.File

        [Display(Name = "직접접근여부")]
        public bool IsProtect { get; set; } //true = url 로 직접 접근할 수 없는 경로에 저장하고 stream으로 읽어와 제공(사용자권한등 체크가 가능해짐)  | false = url 로 바로 접근

        [Display(Name = "Url경로")]
        [StringLength(50, ErrorMessage = "{0}에 유효한 값이 아닙니다.")]
        public string FileUrl { get; set; } //예 : /Data/20200505/, Thumbnail등.. 해당경로에 자동으로 생성되는 파일이 존재할 수 있으므로 이 속성을 이용함

        [Display(Name = "Url전체경로")]
        [StringLength(120, ErrorMessage = "{0}에 유효한 값이 아닙니다.")]
        public string FileFullUrl { get; set; } //예 : /Data/20200505/this.File

        [Display(Name = "파일 정렬 순서")]
        public int? Sort { get; set; }
    }

    public class AttachmentParams
    {
        public int Id { get; set; }


        [Display(Name = "고유키")] //Identity를 이용한 참조를 이용하며 Id를 이용한 참조는 없게 만든다.. 테스트서버, 실서버 생성되는 id값이 다를 수 있기 때문에..
        [StringLength(40, ErrorMessage = "{0}에 유효한 값이 아닙니다.")]
        [Required]
        public string Identity { get; set; }


        [Display(Name = "참조테이블")] //어떤 테이블 데이터에 대한 업로드파일인지
        [StringLength(25, ErrorMessage = "{0}에 유효한 값이 아닙니다.")]
        [Required]
        public string ModelName { get; set; }

        [Display(Name = "파일구분")] //모델내에서 업로드된 파일의 구분을 위한 구분자(예 : 프로필사진인지, 대표이미지 인지..구분용)
        [StringLength(25, ErrorMessage = "{0}에 유효한 값이 아닙니다.")]
        public string Kind { get; set; }


        [Display(Name = "직접접근여부")]
        public bool IsProtect { get; set; }

        [Display(Name = "대표파일")]
        public string DefaultFile { get; set; }







        public bool IsImage { get; set; } //이미지파일여부
        public bool IsPdf { get; set; } = false; //pdf파일 여부
        public int MaxFileCount { get; set; } //최대파일개수
        public int MaxFileSize { get; set; } //최대크기
        public bool AutoUpload { get; set; } //자동업로드 실행
        public string IsListShow { get; set; } = "N";

        public bool IsSort { get; set; } = false; // 이미지 순서 설정 여부

        public bool IsPerforMance { get; set; } = false;
    }


    [Table("VehicleJson")]
    public class VehicleJson
    {
        [Key]
        public string I_SGOODSNO { get; set; }        
        public string I_SAUTOBRAND { get; set; }
        public string I_SAUTOBRANDNM { get; set; }
        public string I_SAUTOCLASS { get; set; }
        public string I_SAUTOCLASSNM { get; set; }
        public string I_SMNGBROFCD { get; set; }
        public string I_SMNGBROFNM { get; set; }
        public string I_SSTCSTSGCD { get; set; }

        public string I_SSTCSTSGCDNM { get; set; }

        public string I_SAUTOMODEL { get; set; }

        public string I_SAUTOMODELNM { get; set; }
        public string I_SYEARSEQ { get; set; }

        public string I_SYEARNM { get; set; }
        public string I_SFRREGYMD { get; set; }
        public string I_SAUTOCOLOR { get; set; }

        public string I_SAUTOCOLORNM { get; set; }

        public string I_SCERTYN { get; set; }

        public string I_SCERTYNNM { get; set; }

        public string I_SPURTPGCD { get; set; } //'03' = 시승차(데모카)

        public string I_SPURTPGCDNM { get; set; }

        public string Name { get; set; }

        public string FuelType { get; set; }
        public string FuelName { get; set; }

        public string UseYN { get; set; }
        
        public string ExtClrCode { get; set; }

        public string ExtClrName { get; set; } //view
        public string AttachmentIdentity { get; set; }

        public string HotYN { get; set; }

        public string AmgPackageYN { get; set; }

        public string SClassPackageYN { get; set; }

        public string I_SMILEAGE { get; set; }

        public decimal? I_SRSPAMT { get; set; }
        
        public string I_PSYN { get; set; }
        public string EngineDriveType { get; set; }
        public string EngineDriveName { get; set; }
        public string I_SSTCSALGCD { get; set; }
        public string I_SSTCSALGNM { get; set; }

        [NotMapped]
        public string Images { get; set; }
    }