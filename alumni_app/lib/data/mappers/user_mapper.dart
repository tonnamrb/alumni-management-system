import '../../domain/entities/user.dart';
import '../models/user_data_model.dart';

/// Mapper to convert between UserDataModel and User entity
class UserMapper {
  /// Convert UserDataModel to User entity
  static User toEntity(UserDataModel model) {
    return User(
      id: model.id,
      name: model.name,
      email: model.email,
      mobilePhone: model.mobilePhone,
      avatar: model.avatar,
      role: model.role,
      isActive: model.isActive,
      createdAt: DateTime.parse(model.createdAt),
      updatedAt: model.updatedAt != null ? DateTime.parse(model.updatedAt!) : null,
    );
  }
  
  /// Convert User entity to UserDataModel
  static UserDataModel toModel(User entity) {
    return UserDataModel(
      id: entity.id,
      name: entity.name,
      email: entity.email,
      mobilePhone: entity.mobilePhone,
      avatar: entity.avatar,
      role: entity.role,
      isActive: entity.isActive,
      createdAt: entity.createdAt.toIso8601String(),
      updatedAt: entity.updatedAt?.toIso8601String(),
    );
  }
}