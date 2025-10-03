/// Data models for Profiles API responses

enum PrivacyLevel { public, friendsOnly, private }

class ProfileDataModel {
  final int id;
  final int userId;
  final String? bio;
  final String? profileImageUrl;
  final String? coverImageUrl;
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
  final List<String> skills;
  final List<String> interests;
  final String? linkedInUrl;
  final String? facebookUrl;
  final String? websiteUrl;
  final String? githubUrl;
  final ProfilePrivacySettingsModel privacySettings;
  final DateTime createdAt;
  final DateTime updatedAt;
  final bool isProfileComplete;
  final int completenessPercentage;

  ProfileDataModel({
    required this.id,
    required this.userId,
    this.bio,
    this.profileImageUrl,
    this.coverImageUrl,
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
    required this.skills,
    required this.interests,
    this.linkedInUrl,
    this.facebookUrl,
    this.websiteUrl,
    this.githubUrl,
    required this.privacySettings,
    required this.createdAt,
    required this.updatedAt,
    required this.isProfileComplete,
    required this.completenessPercentage,
  });

  factory ProfileDataModel.fromJson(Map<String, dynamic> json) {
    return ProfileDataModel(
      id: json['id'] as int,
      userId: json['userId'] as int,
      bio: json['bio'] as String?,
      profileImageUrl: json['profileImageUrl'] as String?,
      coverImageUrl: json['coverImageUrl'] as String?,
      phoneNumber: json['phoneNumber'] as String?,
      dateOfBirth: json['dateOfBirth'] != null 
          ? DateTime.parse(json['dateOfBirth'] as String)
          : null,
      graduationYear: json['graduationYear'] as String?,
      major: json['major'] as String?,
      department: json['department'] as String?,
      faculty: json['faculty'] as String?,
      studentId: json['studentId'] as String?,
      company: json['company'] as String?,
      position: json['position'] as String?,
      workLocation: json['workLocation'] as String?,
      industry: json['industry'] as String?,
      skills: (json['skills'] as List<dynamic>?)
          ?.map((skill) => skill as String)
          .toList() ?? [],
      interests: (json['interests'] as List<dynamic>?)
          ?.map((interest) => interest as String)
          .toList() ?? [],
      linkedInUrl: json['linkedInUrl'] as String?,
      facebookUrl: json['facebookUrl'] as String?,
      websiteUrl: json['websiteUrl'] as String?,
      githubUrl: json['githubUrl'] as String?,
      privacySettings: ProfilePrivacySettingsModel.fromJson(
        json['privacySettings'] as Map<String, dynamic>? ?? {}
      ),
      createdAt: DateTime.parse(json['createdAt'] as String),
      updatedAt: DateTime.parse(json['updatedAt'] as String),
      isProfileComplete: json['isProfileComplete'] as bool? ?? false,
      completenessPercentage: json['completenessPercentage'] as int? ?? 0,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'userId': userId,
      'bio': bio,
      'profileImageUrl': profileImageUrl,
      'coverImageUrl': coverImageUrl,
      'phoneNumber': phoneNumber,
      'dateOfBirth': dateOfBirth?.toIso8601String(),
      'graduationYear': graduationYear,
      'major': major,
      'department': department,
      'faculty': faculty,
      'studentId': studentId,
      'company': company,
      'position': position,
      'workLocation': workLocation,
      'industry': industry,
      'skills': skills,
      'interests': interests,
      'linkedInUrl': linkedInUrl,
      'facebookUrl': facebookUrl,
      'websiteUrl': websiteUrl,
      'githubUrl': githubUrl,
      'privacySettings': privacySettings.toJson(),
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
      'isProfileComplete': isProfileComplete,
      'completenessPercentage': completenessPercentage,
    };
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ProfileDataModel &&
          runtimeType == other.runtimeType &&
          id == other.id;

  @override
  int get hashCode => id.hashCode;

  ProfileDataModel copyWith({
    int? id,
    int? userId,
    String? bio,
    String? profileImageUrl,
    String? coverImageUrl,
    String? phoneNumber,
    DateTime? dateOfBirth,
    String? graduationYear,
    String? major,
    String? department,
    String? faculty,
    String? studentId,
    String? company,
    String? position,
    String? workLocation,
    String? industry,
    List<String>? skills,
    List<String>? interests,
    String? linkedInUrl,
    String? facebookUrl,
    String? websiteUrl,
    String? githubUrl,
    ProfilePrivacySettingsModel? privacySettings,
    DateTime? createdAt,
    DateTime? updatedAt,
    bool? isProfileComplete,
    int? completenessPercentage,
  }) {
    return ProfileDataModel(
      id: id ?? this.id,
      userId: userId ?? this.userId,
      bio: bio ?? this.bio,
      profileImageUrl: profileImageUrl ?? this.profileImageUrl,
      coverImageUrl: coverImageUrl ?? this.coverImageUrl,
      phoneNumber: phoneNumber ?? this.phoneNumber,
      dateOfBirth: dateOfBirth ?? this.dateOfBirth,
      graduationYear: graduationYear ?? this.graduationYear,
      major: major ?? this.major,
      department: department ?? this.department,
      faculty: faculty ?? this.faculty,
      studentId: studentId ?? this.studentId,
      company: company ?? this.company,
      position: position ?? this.position,
      workLocation: workLocation ?? this.workLocation,
      industry: industry ?? this.industry,
      skills: skills ?? this.skills,
      interests: interests ?? this.interests,
      linkedInUrl: linkedInUrl ?? this.linkedInUrl,
      facebookUrl: facebookUrl ?? this.facebookUrl,
      websiteUrl: websiteUrl ?? this.websiteUrl,
      githubUrl: githubUrl ?? this.githubUrl,
      privacySettings: privacySettings ?? this.privacySettings,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
      isProfileComplete: isProfileComplete ?? this.isProfileComplete,
      completenessPercentage: completenessPercentage ?? this.completenessPercentage,
    );
  }
}

/// Model for profile privacy settings
class ProfilePrivacySettingsModel {
  final PrivacyLevel profileVisibility;
  final PrivacyLevel contactInfoVisibility;
  final PrivacyLevel educationVisibility;
  final PrivacyLevel workInfoVisibility;
  final PrivacyLevel socialLinksVisibility;
  final bool allowMessagesFromStrangers;
  final bool showOnlineStatus;
  final bool allowProfileIndexing;

  ProfilePrivacySettingsModel({
    required this.profileVisibility,
    required this.contactInfoVisibility,
    required this.educationVisibility,
    required this.workInfoVisibility,
    required this.socialLinksVisibility,
    required this.allowMessagesFromStrangers,
    required this.showOnlineStatus,
    required this.allowProfileIndexing,
  });

  factory ProfilePrivacySettingsModel.fromJson(Map<String, dynamic> json) {
    return ProfilePrivacySettingsModel(
      profileVisibility: PrivacyLevel.values.firstWhere(
        (e) => e.name == json['profileVisibility'],
        orElse: () => PrivacyLevel.public,
      ),
      contactInfoVisibility: PrivacyLevel.values.firstWhere(
        (e) => e.name == json['contactInfoVisibility'],
        orElse: () => PrivacyLevel.private,
      ),
      educationVisibility: PrivacyLevel.values.firstWhere(
        (e) => e.name == json['educationVisibility'],
        orElse: () => PrivacyLevel.public,
      ),
      workInfoVisibility: PrivacyLevel.values.firstWhere(
        (e) => e.name == json['workInfoVisibility'],
        orElse: () => PrivacyLevel.public,
      ),
      socialLinksVisibility: PrivacyLevel.values.firstWhere(
        (e) => e.name == json['socialLinksVisibility'],
        orElse: () => PrivacyLevel.public,
      ),
      allowMessagesFromStrangers: json['allowMessagesFromStrangers'] as bool? ?? false,
      showOnlineStatus: json['showOnlineStatus'] as bool? ?? true,
      allowProfileIndexing: json['allowProfileIndexing'] as bool? ?? true,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'profileVisibility': profileVisibility.name,
      'contactInfoVisibility': contactInfoVisibility.name,
      'educationVisibility': educationVisibility.name,
      'workInfoVisibility': workInfoVisibility.name,
      'socialLinksVisibility': socialLinksVisibility.name,
      'allowMessagesFromStrangers': allowMessagesFromStrangers,
      'showOnlineStatus': showOnlineStatus,
      'allowProfileIndexing': allowProfileIndexing,
    };
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ProfilePrivacySettingsModel &&
          runtimeType == other.runtimeType &&
          profileVisibility == other.profileVisibility &&
          contactInfoVisibility == other.contactInfoVisibility;

  @override
  int get hashCode => 
      profileVisibility.hashCode ^ contactInfoVisibility.hashCode;
}

/// Model for paginated profile lists
class ProfileListDataModel {
  final List<ProfileDataModel> profiles;
  final int totalCount;
  final int totalPages;
  final int currentPage;
  final int pageSize;
  final bool hasNextPage;
  final bool hasPreviousPage;

  ProfileListDataModel({
    required this.profiles,
    required this.totalCount,
    required this.totalPages,
    required this.currentPage,
    required this.pageSize,
    required this.hasNextPage,
    required this.hasPreviousPage,
  });

  factory ProfileListDataModel.fromJson(Map<String, dynamic> json) {
    return ProfileListDataModel(
      profiles: (json['items'] as List<dynamic>?)
              ?.map((item) => ProfileDataModel.fromJson(item as Map<String, dynamic>))
              .toList() 
          ?? [],
      totalCount: json['totalCount'] as int? ?? 0,
      totalPages: json['totalPages'] as int? ?? 0,
      currentPage: json['currentPage'] as int? ?? 1,
      pageSize: json['pageSize'] as int? ?? 10,
      hasNextPage: json['hasNextPage'] as bool? ?? false,
      hasPreviousPage: json['hasPreviousPage'] as bool? ?? false,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'items': profiles.map((profile) => profile.toJson()).toList(),
      'totalCount': totalCount,
      'totalPages': totalPages,
      'currentPage': currentPage,
      'pageSize': pageSize,
      'hasNextPage': hasNextPage,
      'hasPreviousPage': hasPreviousPage,
    };
  }
}

/// Model for profile completeness information
class ProfileCompletenessModel {
  final int percentage;
  final List<String> missingFields;
  final List<String> completedFields;
  final Map<String, bool> fieldStatus;

  ProfileCompletenessModel({
    required this.percentage,
    required this.missingFields,
    required this.completedFields,
    required this.fieldStatus,
  });

  factory ProfileCompletenessModel.fromJson(Map<String, dynamic> json) {
    return ProfileCompletenessModel(
      percentage: json['percentage'] as int? ?? 0,
      missingFields: (json['missingFields'] as List<dynamic>?)
          ?.map((field) => field as String)
          .toList() ?? [],
      completedFields: (json['completedFields'] as List<dynamic>?)
          ?.map((field) => field as String)
          .toList() ?? [],
      fieldStatus: (json['fieldStatus'] as Map<String, dynamic>?)
          ?.map((key, value) => MapEntry(key, value as bool)) ?? {},
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'percentage': percentage,
      'missingFields': missingFields,
      'completedFields': completedFields,
      'fieldStatus': fieldStatus,
    };
  }
}