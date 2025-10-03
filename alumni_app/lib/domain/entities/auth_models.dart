class AuthResult {
  final String accessToken;
  final String refreshToken;
  final DateTime expiresAt;
  final User user;

  const AuthResult({
    required this.accessToken,
    required this.refreshToken,
    required this.expiresAt,
    required this.user,
  });

  factory AuthResult.fromJson(Map<String, dynamic> json) {
    return AuthResult(
      // รองรับทั้ง 'token' และ 'accessToken' field names
      accessToken: (json['token'] ?? json['accessToken']) as String,
      // ใช้ empty string ถ้าไม่มี refreshToken 
      refreshToken: (json['refreshToken'] ?? '') as String,
      expiresAt: DateTime.parse(json['expiresAt'] as String),
      user: User.fromJson(json['user'] as Map<String, dynamic>),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'accessToken': accessToken,
      'refreshToken': refreshToken,
      'expiresAt': expiresAt.toIso8601String(),
      'user': user.toJson(),
    };
  }
}

class User {
  final int id;
  final String email;
  final String firstname;
  final String lastname;
  final String fullName;
  final String? mobilePhone;
  final int roleId;
  final String? roleName;
  final bool isMember;
  final bool isAdmin;
  final AlumniProfile? alumniProfile;
  final DateTime createdAt;
  final DateTime updatedAt;

  const User({
    required this.id,
    required this.email,
    required this.firstname,
    required this.lastname,
    required this.fullName,
    this.mobilePhone,
    required this.roleId,
    this.roleName,
    required this.isMember,
    required this.isAdmin,
    this.alumniProfile,
    required this.createdAt,
    required this.updatedAt,
  });

  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      id: json['id'] as int,
      email: json['email'] as String? ?? '',
      firstname: json['firstname'] as String? ?? '',
      lastname: json['lastname'] as String? ?? '', 
      fullName: json['fullName'] as String? ?? '${json['firstname'] ?? ''} ${json['lastname'] ?? ''}'.trim(),
      mobilePhone: json['mobilePhone'] as String?,
      roleId: json['roleId'] is String 
          ? int.parse(json['roleId'] as String) 
          : json['roleId'] as int,
      roleName: json['roleName'] as String?,
      isMember: json['isMember'] as bool? ?? false,
      isAdmin: json['isAdmin'] as bool? ?? false,
      alumniProfile: json['alumniProfile'] != null
          ? AlumniProfile.fromJson(json['alumniProfile'] as Map<String, dynamic>)
          : null,
      createdAt: DateTime.parse(json['createdAt'] as String),
      updatedAt: json['updatedAt'] != null 
          ? DateTime.parse(json['updatedAt'] as String)
          : DateTime.now(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'email': email,
      'firstname': firstname,
      'lastname': lastname,
      'fullName': fullName,
      'mobilePhone': mobilePhone,
      'roleId': roleId,
      'roleName': roleName,
      'isMember': isMember,
      'isAdmin': isAdmin,
      'alumniProfile': alumniProfile?.toJson(),
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
    };
  }

  User copyWith({
    int? id,
    String? email,
    String? firstname,
    String? lastname,
    String? fullName,
    String? mobilePhone,
    int? roleId,
    String? roleName,
    bool? isMember,
    bool? isAdmin,
    AlumniProfile? alumniProfile,
    DateTime? createdAt,
    DateTime? updatedAt,
  }) {
    return User(
      id: id ?? this.id,
      email: email ?? this.email,
      firstname: firstname ?? this.firstname,
      lastname: lastname ?? this.lastname,
      fullName: fullName ?? this.fullName,
      mobilePhone: mobilePhone ?? this.mobilePhone,
      roleId: roleId ?? this.roleId,
      roleName: roleName ?? this.roleName,
      isMember: isMember ?? this.isMember,
      isAdmin: isAdmin ?? this.isAdmin,
      alumniProfile: alumniProfile ?? this.alumniProfile,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }
}

class AlumniProfile {
  final int id;
  final int userId;
  final String? bio;
  final String? profilePictureUrl;
  final int? graduationYear;
  final String? major;
  final String? currentJobTitle;
  final String? currentCompany;
  final String? phoneNumber;
  final bool isProfilePublic;
  final String? memberID;
  final String? nameInYearbook;
  final String? groupID;
  final String? lineID;
  final String? facebook;
  final DateTime createdAt;
  final DateTime updatedAt;

  const AlumniProfile({
    required this.id,
    required this.userId,
    this.bio,
    this.profilePictureUrl,
    this.graduationYear,
    this.major,
    this.currentJobTitle,
    this.currentCompany,
    this.phoneNumber,
    required this.isProfilePublic,
    this.memberID,
    this.nameInYearbook,
    this.groupID,
    this.lineID,
    this.facebook,
    required this.createdAt,
    required this.updatedAt,
  });

  factory AlumniProfile.fromJson(Map<String, dynamic> json) {
    return AlumniProfile(
      id: json['id'] as int,
      userId: json['userId'] as int,
      bio: json['bio'] as String?,
      profilePictureUrl: json['profilePictureUrl'] as String?,
      graduationYear: json['graduationYear'] as int?,
      major: json['major'] as String?,
      currentJobTitle: json['currentJobTitle'] as String?,
      currentCompany: json['currentCompany'] as String?,
      phoneNumber: json['phoneNumber'] as String?,
      isProfilePublic: json['isProfilePublic'] as bool,
      memberID: json['memberID'] as String?,
      nameInYearbook: json['nameInYearbook'] as String?,
      groupID: json['groupID'] as String?,
      lineID: json['lineID'] as String?,
      facebook: json['facebook'] as String?,
      createdAt: DateTime.parse(json['createdAt'] as String),
      updatedAt: DateTime.parse(json['updatedAt'] as String),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'userId': userId,
      'bio': bio,
      'profilePictureUrl': profilePictureUrl,
      'graduationYear': graduationYear,
      'major': major,
      'currentJobTitle': currentJobTitle,
      'currentCompany': currentCompany,
      'phoneNumber': phoneNumber,
      'isProfilePublic': isProfilePublic,
      'memberID': memberID,
      'nameInYearbook': nameInYearbook,
      'groupID': groupID,
      'lineID': lineID,
      'facebook': facebook,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
    };
  }
}

enum UserRole {
  member(id: 1, name: 'Member'),
  admin(id: 2, name: 'Administrator');

  const UserRole({required this.id, required this.name});
  
  final int id;
  final String name;

  static UserRole fromId(int id) {
    return UserRole.values.firstWhere(
      (role) => role.id == id,
      orElse: () => UserRole.member,
    );
  }
}