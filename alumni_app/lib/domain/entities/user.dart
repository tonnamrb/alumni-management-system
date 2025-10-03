/// User entity representing a user in the system
class User {
  const User({
    required this.id,
    required this.name,
    required this.email,
    this.mobilePhone,
    this.avatar,
    required this.role,
    required this.isActive,
    required this.createdAt,
    this.updatedAt,
  });
  
  final int id;
  final String name;
  final String email;
  final String? mobilePhone;
  final String? avatar;
  final String role;
  final bool isActive;
  final DateTime createdAt;
  final DateTime? updatedAt;
  
  @override
  bool operator ==(Object other) =>
    identical(this, other) ||
    (other is User &&
      other.id == id &&
      other.name == name &&
      other.email == email &&
      other.mobilePhone == mobilePhone &&
      other.avatar == avatar &&
      other.role == role &&
      other.isActive == isActive &&
      other.createdAt == createdAt &&
      other.updatedAt == updatedAt);
      
  @override
  int get hashCode => Object.hash(
    id,
    name,
    email,
    mobilePhone,
    avatar,
    role,
    isActive,
    createdAt,
    updatedAt,
  );
  
  @override
  String toString() => 'User(id: $id, name: $name, email: $email)';

  /// Convert to JSON
  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'email': email,
      'mobilePhone': mobilePhone,
      'avatar': avatar,
      'role': role,
      'isActive': isActive,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt?.toIso8601String(),
    };
  }

  /// Create from JSON
  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      id: json['id'] as int,
      name: json['name'] as String,
      email: json['email'] as String,
      mobilePhone: json['mobilePhone'] as String?,
      avatar: json['avatar'] as String?,
      role: json['role'] as String? ?? 'user',
      isActive: json['isActive'] as bool? ?? true,
      createdAt: DateTime.parse(json['createdAt'] as String),
      updatedAt: json['updatedAt'] != null ? DateTime.parse(json['updatedAt'] as String) : null,
    );
  }
  
  /// Create a copy with updated fields
  User copyWith({
    int? id,
    String? name,
    String? email,
    String? mobilePhone,
    String? avatar,
    String? role,
    bool? isActive,
    DateTime? createdAt,
    DateTime? updatedAt,
  }) {
    return User(
      id: id ?? this.id,
      name: name ?? this.name,
      email: email ?? this.email,
      mobilePhone: mobilePhone ?? this.mobilePhone,
      avatar: avatar ?? this.avatar,
      role: role ?? this.role,
      isActive: isActive ?? this.isActive,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }
}