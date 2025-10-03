/// Data models for Reports API responses

enum ReportType { 
  spam, 
  harassment, 
  inappropriate, 
  copyright, 
  falseInfo, 
  violence, 
  hate, 
  other 
}

enum ReportStatus { 
  pending, 
  underReview, 
  resolved, 
  dismissed, 
  escalated 
}

enum ContentType { 
  post, 
  comment, 
  user, 
  profile 
}

class ReportDataModel {
  final int id;
  final ReportType reportType;
  final ContentType contentType;
  final int contentId;
  final String contentTitle;
  final String? contentUrl;
  final int reportedByUserId;
  final String reportedByUserName;
  final int? reportedUserId;
  final String? reportedUserName;
  final String reason;
  final String? additionalInfo;
  final ReportStatus status;
  final int? assignedToAdminId;
  final String? assignedToAdminName;
  final List<ReportCommentModel> adminComments;
  final DateTime createdAt;
  final DateTime updatedAt;
  final DateTime? resolvedAt;
  final String? resolutionNote;
  final Map<String, dynamic>? metadata;

  ReportDataModel({
    required this.id,
    required this.reportType,
    required this.contentType,
    required this.contentId,
    required this.contentTitle,
    this.contentUrl,
    required this.reportedByUserId,
    required this.reportedByUserName,
    this.reportedUserId,
    this.reportedUserName,
    required this.reason,
    this.additionalInfo,
    required this.status,
    this.assignedToAdminId,
    this.assignedToAdminName,
    required this.adminComments,
    required this.createdAt,
    required this.updatedAt,
    this.resolvedAt,
    this.resolutionNote,
    this.metadata,
  });

  factory ReportDataModel.fromJson(Map<String, dynamic> json) {
    return ReportDataModel(
      id: json['id'] as int,
      reportType: ReportType.values.firstWhere(
        (e) => e.name == json['reportType'],
        orElse: () => ReportType.other,
      ),
      contentType: ContentType.values.firstWhere(
        (e) => e.name == json['contentType'],
        orElse: () => ContentType.post,
      ),
      contentId: json['contentId'] as int,
      contentTitle: json['contentTitle'] as String,
      contentUrl: json['contentUrl'] as String?,
      reportedByUserId: json['reportedByUserId'] as int,
      reportedByUserName: json['reportedByUserName'] as String,
      reportedUserId: json['reportedUserId'] as int?,
      reportedUserName: json['reportedUserName'] as String?,
      reason: json['reason'] as String,
      additionalInfo: json['additionalInfo'] as String?,
      status: ReportStatus.values.firstWhere(
        (e) => e.name == json['status'],
        orElse: () => ReportStatus.pending,
      ),
      assignedToAdminId: json['assignedToAdminId'] as int?,
      assignedToAdminName: json['assignedToAdminName'] as String?,
      adminComments: (json['adminComments'] as List<dynamic>?)
              ?.map((comment) => ReportCommentModel.fromJson(comment as Map<String, dynamic>))
              .toList() 
          ?? [],
      createdAt: DateTime.parse(json['createdAt'] as String),
      updatedAt: DateTime.parse(json['updatedAt'] as String),
      resolvedAt: json['resolvedAt'] != null
          ? DateTime.parse(json['resolvedAt'] as String)
          : null,
      resolutionNote: json['resolutionNote'] as String?,
      metadata: json['metadata'] as Map<String, dynamic>?,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'reportType': reportType.name,
      'contentType': contentType.name,
      'contentId': contentId,
      'contentTitle': contentTitle,
      'contentUrl': contentUrl,
      'reportedByUserId': reportedByUserId,
      'reportedByUserName': reportedByUserName,
      'reportedUserId': reportedUserId,
      'reportedUserName': reportedUserName,
      'reason': reason,
      'additionalInfo': additionalInfo,
      'status': status.name,
      'assignedToAdminId': assignedToAdminId,
      'assignedToAdminName': assignedToAdminName,
      'adminComments': adminComments.map((comment) => comment.toJson()).toList(),
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
      'resolvedAt': resolvedAt?.toIso8601String(),
      'resolutionNote': resolutionNote,
      'metadata': metadata,
    };
  }

  bool get isResolved => status == ReportStatus.resolved;
  bool get isPending => status == ReportStatus.pending;
  bool get isUnderReview => status == ReportStatus.underReview;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ReportDataModel &&
          runtimeType == other.runtimeType &&
          id == other.id;

  @override
  int get hashCode => id.hashCode;

  ReportDataModel copyWith({
    int? id,
    ReportType? reportType,
    ContentType? contentType,
    int? contentId,
    String? contentTitle,
    String? contentUrl,
    int? reportedByUserId,
    String? reportedByUserName,
    int? reportedUserId,
    String? reportedUserName,
    String? reason,
    String? additionalInfo,
    ReportStatus? status,
    int? assignedToAdminId,
    String? assignedToAdminName,
    List<ReportCommentModel>? adminComments,
    DateTime? createdAt,
    DateTime? updatedAt,
    DateTime? resolvedAt,
    String? resolutionNote,
    Map<String, dynamic>? metadata,
  }) {
    return ReportDataModel(
      id: id ?? this.id,
      reportType: reportType ?? this.reportType,
      contentType: contentType ?? this.contentType,
      contentId: contentId ?? this.contentId,
      contentTitle: contentTitle ?? this.contentTitle,
      contentUrl: contentUrl ?? this.contentUrl,
      reportedByUserId: reportedByUserId ?? this.reportedByUserId,
      reportedByUserName: reportedByUserName ?? this.reportedByUserName,
      reportedUserId: reportedUserId ?? this.reportedUserId,
      reportedUserName: reportedUserName ?? this.reportedUserName,
      reason: reason ?? this.reason,
      additionalInfo: additionalInfo ?? this.additionalInfo,
      status: status ?? this.status,
      assignedToAdminId: assignedToAdminId ?? this.assignedToAdminId,
      assignedToAdminName: assignedToAdminName ?? this.assignedToAdminName,
      adminComments: adminComments ?? this.adminComments,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
      resolvedAt: resolvedAt ?? this.resolvedAt,
      resolutionNote: resolutionNote ?? this.resolutionNote,
      metadata: metadata ?? this.metadata,
    );
  }
}

/// Model for admin comments on reports
class ReportCommentModel {
  final int id;
  final int reportId;
  final int adminId;
  final String adminName;
  final String comment;
  final DateTime createdAt;

  ReportCommentModel({
    required this.id,
    required this.reportId,
    required this.adminId,
    required this.adminName,
    required this.comment,
    required this.createdAt,
  });

  factory ReportCommentModel.fromJson(Map<String, dynamic> json) {
    return ReportCommentModel(
      id: json['id'] as int,
      reportId: json['reportId'] as int,
      adminId: json['adminId'] as int,
      adminName: json['adminName'] as String,
      comment: json['comment'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'reportId': reportId,
      'adminId': adminId,
      'adminName': adminName,
      'comment': comment,
      'createdAt': createdAt.toIso8601String(),
    };
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ReportCommentModel &&
          runtimeType == other.runtimeType &&
          id == other.id;

  @override
  int get hashCode => id.hashCode;
}

/// Model for paginated report lists
class ReportListDataModel {
  final List<ReportDataModel> reports;
  final int totalCount;
  final int totalPages;
  final int currentPage;
  final int pageSize;
  final bool hasNextPage;
  final bool hasPreviousPage;
  final Map<ReportStatus, int> statusBreakdown;
  final Map<ReportType, int> typeBreakdown;

  ReportListDataModel({
    required this.reports,
    required this.totalCount,
    required this.totalPages,
    required this.currentPage,
    required this.pageSize,
    required this.hasNextPage,
    required this.hasPreviousPage,
    required this.statusBreakdown,
    required this.typeBreakdown,
  });

  factory ReportListDataModel.fromJson(Map<String, dynamic> json) {
    final statusBreakdownMap = <ReportStatus, int>{};
    final statusBreakdownJson = json['statusBreakdown'] as Map<String, dynamic>? ?? {};
    
    for (final entry in statusBreakdownJson.entries) {
      final status = ReportStatus.values.firstWhere(
        (e) => e.name == entry.key,
        orElse: () => ReportStatus.pending,
      );
      statusBreakdownMap[status] = entry.value as int;
    }

    final typeBreakdownMap = <ReportType, int>{};
    final typeBreakdownJson = json['typeBreakdown'] as Map<String, dynamic>? ?? {};
    
    for (final entry in typeBreakdownJson.entries) {
      final type = ReportType.values.firstWhere(
        (e) => e.name == entry.key,
        orElse: () => ReportType.other,
      );
      typeBreakdownMap[type] = entry.value as int;
    }

    return ReportListDataModel(
      reports: (json['items'] as List<dynamic>?)
              ?.map((item) => ReportDataModel.fromJson(item as Map<String, dynamic>))
              .toList() 
          ?? [],
      totalCount: json['totalCount'] as int? ?? 0,
      totalPages: json['totalPages'] as int? ?? 0,
      currentPage: json['currentPage'] as int? ?? 1,
      pageSize: json['pageSize'] as int? ?? 10,
      hasNextPage: json['hasNextPage'] as bool? ?? false,
      hasPreviousPage: json['hasPreviousPage'] as bool? ?? false,
      statusBreakdown: statusBreakdownMap,
      typeBreakdown: typeBreakdownMap,
    );
  }

  Map<String, dynamic> toJson() {
    final statusBreakdownJson = <String, dynamic>{};
    for (final entry in statusBreakdown.entries) {
      statusBreakdownJson[entry.key.name] = entry.value;
    }

    final typeBreakdownJson = <String, dynamic>{};
    for (final entry in typeBreakdown.entries) {
      typeBreakdownJson[entry.key.name] = entry.value;
    }

    return {
      'items': reports.map((report) => report.toJson()).toList(),
      'totalCount': totalCount,
      'totalPages': totalPages,
      'currentPage': currentPage,
      'pageSize': pageSize,
      'hasNextPage': hasNextPage,
      'hasPreviousPage': hasPreviousPage,
      'statusBreakdown': statusBreakdownJson,
      'typeBreakdown': typeBreakdownJson,
    };
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ReportListDataModel &&
          runtimeType == other.runtimeType &&
          reports == other.reports &&
          currentPage == other.currentPage;

  @override
  int get hashCode => reports.hashCode ^ currentPage.hashCode;
}

/// Model for report statistics
class ReportStatsDataModel {
  final int totalReports;
  final int pendingReports;
  final int resolvedReports;
  final int dismissedReports;
  final Map<ReportType, int> reportsByType;
  final Map<String, int> reportsByMonth;
  final double averageResolutionTime;
  final int mostReportedContentId;
  final String mostReportedContentType;

  ReportStatsDataModel({
    required this.totalReports,
    required this.pendingReports,
    required this.resolvedReports,
    required this.dismissedReports,
    required this.reportsByType,
    required this.reportsByMonth,
    required this.averageResolutionTime,
    required this.mostReportedContentId,
    required this.mostReportedContentType,
  });

  factory ReportStatsDataModel.fromJson(Map<String, dynamic> json) {
    final reportsByTypeMap = <ReportType, int>{};
    final reportsByTypeJson = json['reportsByType'] as Map<String, dynamic>? ?? {};
    
    for (final entry in reportsByTypeJson.entries) {
      final type = ReportType.values.firstWhere(
        (e) => e.name == entry.key,
        orElse: () => ReportType.other,
      );
      reportsByTypeMap[type] = entry.value as int;
    }

    return ReportStatsDataModel(
      totalReports: json['totalReports'] as int? ?? 0,
      pendingReports: json['pendingReports'] as int? ?? 0,
      resolvedReports: json['resolvedReports'] as int? ?? 0,
      dismissedReports: json['dismissedReports'] as int? ?? 0,
      reportsByType: reportsByTypeMap,
      reportsByMonth: (json['reportsByMonth'] as Map<String, dynamic>?)
          ?.map((key, value) => MapEntry(key, value as int)) ?? {},
      averageResolutionTime: (json['averageResolutionTime'] as num?)?.toDouble() ?? 0.0,
      mostReportedContentId: json['mostReportedContentId'] as int? ?? 0,
      mostReportedContentType: json['mostReportedContentType'] as String? ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    final reportsByTypeJson = <String, dynamic>{};
    for (final entry in reportsByType.entries) {
      reportsByTypeJson[entry.key.name] = entry.value;
    }

    return {
      'totalReports': totalReports,
      'pendingReports': pendingReports,
      'resolvedReports': resolvedReports,
      'dismissedReports': dismissedReports,
      'reportsByType': reportsByTypeJson,
      'reportsByMonth': reportsByMonth,
      'averageResolutionTime': averageResolutionTime,
      'mostReportedContentId': mostReportedContentId,
      'mostReportedContentType': mostReportedContentType,
    };
  }

  double get resolutionRate {
    if (totalReports == 0) return 0.0;
    return (resolvedReports / totalReports) * 100;
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ReportStatsDataModel &&
          runtimeType == other.runtimeType &&
          totalReports == other.totalReports &&
          pendingReports == other.pendingReports;

  @override
  int get hashCode => totalReports.hashCode ^ pendingReports.hashCode;
}