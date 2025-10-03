/// Request models for Users API

class UpdateUserProfileRequestModel {
  final String? firstName;
  final String? lastName;
  final String? displayName;
  final String? bio;
  final String? phoneNumber;
  final DateTime? dateOfBirth;
  final String? graduationYear;
  final String? major;
  final String? department;
  final String? company;
  final String? position;

  UpdateUserProfileRequestModel({
    this.firstName,
    this.lastName,
    this.displayName,
    this.bio,
    this.phoneNumber,
    this.dateOfBirth,
    this.graduationYear,
    this.major,
    this.department,
    this.company,
    this.position,
  });

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    
    if (firstName != null) json['firstName'] = firstName;
    if (lastName != null) json['lastName'] = lastName;
    if (displayName != null) json['displayName'] = displayName;
    if (bio != null) json['bio'] = bio;
    if (phoneNumber != null) json['phoneNumber'] = phoneNumber;
    if (dateOfBirth != null) json['dateOfBirth'] = dateOfBirth!.toIso8601String();
    if (graduationYear != null) json['graduationYear'] = graduationYear;
    if (major != null) json['major'] = major;
    if (department != null) json['department'] = department;
    if (company != null) json['company'] = company;
    if (position != null) json['position'] = position;
    
    return json;
  }
}

class ChangePasswordRequestModel {
  final String currentPassword;
  final String newPassword;
  final String confirmPassword;

  ChangePasswordRequestModel({
    required this.currentPassword,
    required this.newPassword,
    required this.confirmPassword,
  });

  Map<String, dynamic> toJson() {
    return {
      'currentPassword': currentPassword,
      'newPassword': newPassword,
      'confirmPassword': confirmPassword,
    };
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ChangePasswordRequestModel &&
          runtimeType == other.runtimeType &&
          currentPassword == other.currentPassword &&
          newPassword == other.newPassword &&
          confirmPassword == other.confirmPassword;

  @override
  int get hashCode =>
      currentPassword.hashCode ^ newPassword.hashCode ^ confirmPassword.hashCode;
}

class UserSearchParams {
  final String? query;
  final String? role;
  final String? status;
  final String? graduationYear;
  final String? major;
  final String? department;
  final int page;
  final int pageSize;
  final String? sortBy;
  final String? sortDirection;

  UserSearchParams({
    this.query,
    this.role,
    this.status,
    this.graduationYear,
    this.major,
    this.department,
    this.page = 1,
    this.pageSize = 10,
    this.sortBy,
    this.sortDirection = 'asc',
  });

  Map<String, dynamic> toQueryParams() {
    final params = <String, dynamic>{
      'page': page,
      'pageSize': pageSize,
    };

    if (query != null && query!.isNotEmpty) params['query'] = query;
    if (role != null) params['role'] = role;
    if (status != null) params['status'] = status;
    if (graduationYear != null) params['graduationYear'] = graduationYear;
    if (major != null) params['major'] = major;
    if (department != null) params['department'] = department;
    if (sortBy != null) params['sortBy'] = sortBy;
    if (sortDirection != null) params['sortDirection'] = sortDirection;

    return params;
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is UserSearchParams &&
          runtimeType == other.runtimeType &&
          query == other.query &&
          role == other.role &&
          status == other.status &&
          graduationYear == other.graduationYear &&
          major == other.major &&
          department == other.department &&
          page == other.page &&
          pageSize == other.pageSize &&
          sortBy == other.sortBy &&
          sortDirection == other.sortDirection;

  @override
  int get hashCode =>
      query.hashCode ^
      role.hashCode ^
      status.hashCode ^
      graduationYear.hashCode ^
      major.hashCode ^
      department.hashCode ^
      page.hashCode ^
      pageSize.hashCode ^
      sortBy.hashCode ^
      sortDirection.hashCode;
}

class UserFilterParams {
  final String? role;
  final String? status;
  final bool? isActive;
  final DateTime? createdAfter;
  final DateTime? createdBefore;
  final String? graduationYear;
  final String? major;
  final String? department;
  final int page;
  final int pageSize;
  final String? sortBy;
  final String? sortDirection;

  UserFilterParams({
    this.role,
    this.status,
    this.isActive,
    this.createdAfter,
    this.createdBefore,
    this.graduationYear,
    this.major,
    this.department,
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

    if (role != null) params['role'] = role;
    if (status != null) params['status'] = status;
    if (isActive != null) params['isActive'] = isActive;
    if (createdAfter != null) params['createdAfter'] = createdAfter!.toIso8601String();
    if (createdBefore != null) params['createdBefore'] = createdBefore!.toIso8601String();
    if (graduationYear != null) params['graduationYear'] = graduationYear;
    if (major != null) params['major'] = major;
    if (department != null) params['department'] = department;
    if (sortBy != null) params['sortBy'] = sortBy;
    if (sortDirection != null) params['sortDirection'] = sortDirection;

    return params;
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is UserFilterParams &&
          runtimeType == other.runtimeType &&
          role == other.role &&
          status == other.status &&
          isActive == other.isActive &&
          createdAfter == other.createdAfter &&
          createdBefore == other.createdBefore &&
          graduationYear == other.graduationYear &&
          major == other.major &&
          department == other.department &&
          page == other.page &&
          pageSize == other.pageSize &&
          sortBy == other.sortBy &&
          sortDirection == other.sortDirection;

  @override
  int get hashCode =>
      role.hashCode ^
      status.hashCode ^
      isActive.hashCode ^
      createdAfter.hashCode ^
      createdBefore.hashCode ^
      graduationYear.hashCode ^
      major.hashCode ^
      department.hashCode ^
      page.hashCode ^
      pageSize.hashCode ^
      sortBy.hashCode ^
      sortDirection.hashCode;
}