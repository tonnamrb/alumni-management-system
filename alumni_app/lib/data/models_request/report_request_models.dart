/// Request models for Reports API

import '../models/report_data_model.dart';

class CreateReportRequestModel {
  final ReportType reportType;
  final ContentType contentType;
  final int contentId;
  final String reason;
  final String? additionalInfo;

  CreateReportRequestModel({
    required this.reportType,
    required this.contentType,
    required this.contentId,
    required this.reason,
    this.additionalInfo,
  });

  Map<String, dynamic> toJson() {
    return {
      'reportType': reportType.name,
      'contentType': contentType.name,
      'contentId': contentId,
      'reason': reason,
      if (additionalInfo != null) 'additionalInfo': additionalInfo,
    };
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is CreateReportRequestModel &&
          runtimeType == other.runtimeType &&
          reportType == other.reportType &&
          contentType == other.contentType &&
          contentId == other.contentId &&
          reason == other.reason &&
          additionalInfo == other.additionalInfo;

  @override
  int get hashCode =>
      reportType.hashCode ^
      contentType.hashCode ^
      contentId.hashCode ^
      reason.hashCode ^
      additionalInfo.hashCode;
}

class UpdateReportStatusRequestModel {
  final ReportStatus status;
  final String? resolutionNote;
  final int? assignedToAdminId;

  UpdateReportStatusRequestModel({
    required this.status,
    this.resolutionNote,
    this.assignedToAdminId,
  });

  Map<String, dynamic> toJson() {
    return {
      'status': status.name,
      if (resolutionNote != null) 'resolutionNote': resolutionNote,
      if (assignedToAdminId != null) 'assignedToAdminId': assignedToAdminId,
    };
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is UpdateReportStatusRequestModel &&
          runtimeType == other.runtimeType &&
          status == other.status &&
          resolutionNote == other.resolutionNote &&
          assignedToAdminId == other.assignedToAdminId;

  @override
  int get hashCode =>
      status.hashCode ^ resolutionNote.hashCode ^ assignedToAdminId.hashCode;
}

class AddReportCommentRequestModel {
  final String comment;

  AddReportCommentRequestModel({
    required this.comment,
  });

  Map<String, dynamic> toJson() {
    return {
      'comment': comment,
    };
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is AddReportCommentRequestModel &&
          runtimeType == other.runtimeType &&
          comment == other.comment;

  @override
  int get hashCode => comment.hashCode;
}

class ReportFilterParams {
  final ReportType? reportType;
  final ReportStatus? status;
  final ContentType? contentType;
  final int? reportedByUserId;
  final int? reportedUserId;
  final int? assignedToAdminId;
  final DateTime? createdAfter;
  final DateTime? createdBefore;
  final DateTime? resolvedAfter;
  final DateTime? resolvedBefore;
  final int page;
  final int pageSize;
  final String? sortBy;
  final String? sortDirection;

  ReportFilterParams({
    this.reportType,
    this.status,
    this.contentType,
    this.reportedByUserId,
    this.reportedUserId,
    this.assignedToAdminId,
    this.createdAfter,
    this.createdBefore,
    this.resolvedAfter,
    this.resolvedBefore,
    this.page = 1,
    this.pageSize = 10,
    this.sortBy,
    this.sortDirection = 'desc',
  });

  Map<String, dynamic> toQueryParams() {
    final params = <String, dynamic>{
      'page': page,
      'pageSize': pageSize,
    };

    if (reportType != null) params['reportType'] = reportType!.name;
    if (status != null) params['status'] = status!.name;
    if (contentType != null) params['contentType'] = contentType!.name;
    if (reportedByUserId != null) params['reportedByUserId'] = reportedByUserId;
    if (reportedUserId != null) params['reportedUserId'] = reportedUserId;
    if (assignedToAdminId != null) params['assignedToAdminId'] = assignedToAdminId;
    if (createdAfter != null) params['createdAfter'] = createdAfter!.toIso8601String();
    if (createdBefore != null) params['createdBefore'] = createdBefore!.toIso8601String();
    if (resolvedAfter != null) params['resolvedAfter'] = resolvedAfter!.toIso8601String();
    if (resolvedBefore != null) params['resolvedBefore'] = resolvedBefore!.toIso8601String();
    if (sortBy != null) params['sortBy'] = sortBy;
    if (sortDirection != null) params['sortDirection'] = sortDirection;

    return params;
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ReportFilterParams &&
          runtimeType == other.runtimeType &&
          reportType == other.reportType &&
          status == other.status &&
          contentType == other.contentType &&
          reportedByUserId == other.reportedByUserId &&
          reportedUserId == other.reportedUserId &&
          assignedToAdminId == other.assignedToAdminId &&
          createdAfter == other.createdAfter &&
          createdBefore == other.createdBefore &&
          resolvedAfter == other.resolvedAfter &&
          resolvedBefore == other.resolvedBefore &&
          page == other.page &&
          pageSize == other.pageSize &&
          sortBy == other.sortBy &&
          sortDirection == other.sortDirection;

  @override
  int get hashCode =>
      reportType.hashCode ^
      status.hashCode ^
      contentType.hashCode ^
      reportedByUserId.hashCode ^
      reportedUserId.hashCode ^
      assignedToAdminId.hashCode ^
      createdAfter.hashCode ^
      createdBefore.hashCode ^
      resolvedAfter.hashCode ^
      resolvedBefore.hashCode ^
      page.hashCode ^
      pageSize.hashCode ^
      sortBy.hashCode ^
      sortDirection.hashCode;
}

class ReportSearchParams {
  final String? query;
  final ReportType? reportType;
  final ReportStatus? status;
  final ContentType? contentType;
  final String? reportedUserName;
  final String? adminName;
  final int page;
  final int pageSize;
  final String? sortBy;
  final String? sortDirection;

  ReportSearchParams({
    this.query,
    this.reportType,
    this.status,
    this.contentType,
    this.reportedUserName,
    this.adminName,
    this.page = 1,
    this.pageSize = 10,
    this.sortBy,
    this.sortDirection = 'desc',
  });

  Map<String, dynamic> toQueryParams() {
    final params = <String, dynamic>{
      'page': page,
      'pageSize': pageSize,
    };

    if (query != null && query!.isNotEmpty) params['query'] = query;
    if (reportType != null) params['reportType'] = reportType!.name;
    if (status != null) params['status'] = status!.name;
    if (contentType != null) params['contentType'] = contentType!.name;
    if (reportedUserName != null && reportedUserName!.isNotEmpty) {
      params['reportedUserName'] = reportedUserName;
    }
    if (adminName != null && adminName!.isNotEmpty) {
      params['adminName'] = adminName;
    }
    if (sortBy != null) params['sortBy'] = sortBy;
    if (sortDirection != null) params['sortDirection'] = sortDirection;

    return params;
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ReportSearchParams &&
          runtimeType == other.runtimeType &&
          query == other.query &&
          reportType == other.reportType &&
          status == other.status &&
          contentType == other.contentType &&
          reportedUserName == other.reportedUserName &&
          adminName == other.adminName &&
          page == other.page &&
          pageSize == other.pageSize &&
          sortBy == other.sortBy &&
          sortDirection == other.sortDirection;

  @override
  int get hashCode =>
      query.hashCode ^
      reportType.hashCode ^
      status.hashCode ^
      contentType.hashCode ^
      reportedUserName.hashCode ^
      adminName.hashCode ^
      page.hashCode ^
      pageSize.hashCode ^
      sortBy.hashCode ^
      sortDirection.hashCode;
}