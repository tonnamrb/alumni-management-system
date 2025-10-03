/// Request models for Profiles API

class UpdateProfileRequestModel {
  final String? bio;
  final String? phoneNumber;
  final DateTime? dateOfBirth;
  final String? graduationYear;
  final String? major;
  final String? department;
  final String? faculty;
  final String? studentId;
  final String? company;
  final String? position;
  final String? workLocation;
  final String? industry;
  final List<String>? skills;
  final List<String>? interests;
  final String? linkedInUrl;
  final String? facebookUrl;
  final String? websiteUrl;
  final String? githubUrl;

  UpdateProfileRequestModel({
    this.bio,
    this.phoneNumber,
    this.dateOfBirth,
    this.graduationYear,
    this.major,
    this.department,
    this.faculty,
    this.studentId,
    this.company,
    this.position,
    this.workLocation,
    this.industry,
    this.skills,
    this.interests,
    this.linkedInUrl,
    this.facebookUrl,
    this.websiteUrl,
    this.githubUrl,
  });

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    
    if (bio != null) json['bio'] = bio;
    if (phoneNumber != null) json['phoneNumber'] = phoneNumber;
    if (dateOfBirth != null) json['dateOfBirth'] = dateOfBirth!.toIso8601String();
    if (graduationYear != null) json['graduationYear'] = graduationYear;
    if (major != null) json['major'] = major;
    if (department != null) json['department'] = department;
    if (faculty != null) json['faculty'] = faculty;
    if (studentId != null) json['studentId'] = studentId;
    if (company != null) json['company'] = company;
    if (position != null) json['position'] = position;
    if (workLocation != null) json['workLocation'] = workLocation;
    if (industry != null) json['industry'] = industry;
    if (skills != null) json['skills'] = skills;
    if (interests != null) json['interests'] = interests;
    if (linkedInUrl != null) json['linkedInUrl'] = linkedInUrl;
    if (facebookUrl != null) json['facebookUrl'] = facebookUrl;
    if (websiteUrl != null) json['websiteUrl'] = websiteUrl;
    if (githubUrl != null) json['githubUrl'] = githubUrl;
    
    return json;
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is UpdateProfileRequestModel &&
          runtimeType == other.runtimeType &&
          bio == other.bio &&
          phoneNumber == other.phoneNumber &&
          graduationYear == other.graduationYear &&
          major == other.major &&
          department == other.department;

  @override
  int get hashCode => 
      bio.hashCode ^ 
      phoneNumber.hashCode ^ 
      graduationYear.hashCode ^ 
      major.hashCode ^ 
      department.hashCode;
}

class ProfileSearchParams {
  final String? query;
  final String? graduationYear;
  final String? major;
  final String? department;
  final String? faculty;
  final String? company;
  final String? industry;
  final List<String>? skills;
  final String? location;
  final int page;
  final int pageSize;
  final String? sortBy;
  final String? sortDirection;

  ProfileSearchParams({
    this.query,
    this.graduationYear,
    this.major,
    this.department,
    this.faculty,
    this.company,
    this.industry,
    this.skills,
    this.location,
    this.page = 1,
    this.pageSize = 20,
    this.sortBy,
    this.sortDirection = 'asc',
  });

  Map<String, dynamic> toQueryParams() {
    final params = <String, dynamic>{
      'page': page,
      'pageSize': pageSize,
    };

    if (query != null && query!.isNotEmpty) params['query'] = query;
    if (graduationYear != null) params['graduationYear'] = graduationYear;
    if (major != null) params['major'] = major;
    if (department != null) params['department'] = department;
    if (faculty != null) params['faculty'] = faculty;
    if (company != null) params['company'] = company;
    if (industry != null) params['industry'] = industry;
    if (skills != null && skills!.isNotEmpty) {
      params['skills'] = skills!.join(',');
    }
    if (location != null) params['location'] = location;
    if (sortBy != null) params['sortBy'] = sortBy;
    if (sortDirection != null) params['sortDirection'] = sortDirection;

    return params;
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ProfileSearchParams &&
          runtimeType == other.runtimeType &&
          query == other.query &&
          graduationYear == other.graduationYear &&
          major == other.major &&
          department == other.department &&
          faculty == other.faculty &&
          company == other.company &&
          industry == other.industry &&
          page == other.page &&
          pageSize == other.pageSize;

  @override
  int get hashCode =>
      query.hashCode ^
      graduationYear.hashCode ^
      major.hashCode ^
      department.hashCode ^
      faculty.hashCode ^
      company.hashCode ^
      industry.hashCode ^
      page.hashCode ^
      pageSize.hashCode;
}

class ProfileFilterParams {
  final String? graduationYear;
  final String? major;
  final String? department;
  final String? faculty;
  final String? company;
  final String? industry;
  final bool? isProfileComplete;
  final DateTime? createdAfter;
  final DateTime? createdBefore;
  final int page;
  final int pageSize;
  final String? sortBy;
  final String? sortDirection;

  ProfileFilterParams({
    this.graduationYear,
    this.major,
    this.department,
    this.faculty,
    this.company,
    this.industry,
    this.isProfileComplete,
    this.createdAfter,
    this.createdBefore,
    this.page = 1,
    this.pageSize = 20,
    this.sortBy,
    this.sortDirection = 'desc',
  });

  Map<String, dynamic> toQueryParams() {
    final params = <String, dynamic>{
      'page': page,
      'pageSize': pageSize,
    };

    if (graduationYear != null) params['graduationYear'] = graduationYear;
    if (major != null) params['major'] = major;
    if (department != null) params['department'] = department;
    if (faculty != null) params['faculty'] = faculty;
    if (company != null) params['company'] = company;
    if (industry != null) params['industry'] = industry;
    if (isProfileComplete != null) params['isProfileComplete'] = isProfileComplete;
    if (createdAfter != null) params['createdAfter'] = createdAfter!.toIso8601String();
    if (createdBefore != null) params['createdBefore'] = createdBefore!.toIso8601String();
    if (sortBy != null) params['sortBy'] = sortBy;
    if (sortDirection != null) params['sortDirection'] = sortDirection;

    return params;
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ProfileFilterParams &&
          runtimeType == other.runtimeType &&
          graduationYear == other.graduationYear &&
          major == other.major &&
          department == other.department &&
          faculty == other.faculty &&
          company == other.company &&
          industry == other.industry &&
          isProfileComplete == other.isProfileComplete &&
          page == other.page &&
          pageSize == other.pageSize;

  @override
  int get hashCode =>
      graduationYear.hashCode ^
      major.hashCode ^
      department.hashCode ^
      faculty.hashCode ^
      company.hashCode ^
      industry.hashCode ^
      isProfileComplete.hashCode ^
      page.hashCode ^
      pageSize.hashCode;
}