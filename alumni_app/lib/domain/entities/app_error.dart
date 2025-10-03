class AppError {
  final ErrorType type;
  final String message;
  final String? technicalDetails;
  final bool canRetry;
  final int? statusCode;

  const AppError({
    required this.type,
    required this.message,
    this.technicalDetails,
    this.canRetry = false,
    this.statusCode,
  });

  factory AppError.network(String message, {String? technicalDetails}) {
    return AppError(
      type: ErrorType.network,
      message: message,
      technicalDetails: technicalDetails,
      canRetry: true,
    );
  }

  factory AppError.authentication(String message, {int? statusCode}) {
    return AppError(
      type: ErrorType.authentication,
      message: message,
      statusCode: statusCode,
    );
  }

  factory AppError.authorization(String message) {
    return AppError(
      type: ErrorType.authorization,
      message: message,
    );
  }

  factory AppError.validation(String message) {
    return AppError(
      type: ErrorType.validation,
      message: message,
    );
  }

  factory AppError.server(String message, {int? statusCode}) {
    return AppError(
      type: ErrorType.server,
      message: message,
      statusCode: statusCode,
      canRetry: true,
    );
  }

  factory AppError.unknown(String message) {
    return AppError(
      type: ErrorType.unknown,
      message: message,
    );
  }

  @override
  String toString() {
    return 'AppError(type: $type, message: $message, statusCode: $statusCode)';
  }
}

enum ErrorType {
  network,
  authentication,
  authorization,
  validation,
  server,
  unknown,
}