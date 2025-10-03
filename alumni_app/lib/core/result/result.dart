// Simple Result type for backward compatibility
abstract class Result<T> {
  const Result();
  
  static Result<T> success<T>(T data) => Success<T>(data);
  static Result<T> failure<T>(dynamic error) => Failure<T>(error);
  
  R fold<R>(
    R Function(T) onSuccess,
    R Function(dynamic) onFailure,
  );
  
  // Add when method for compatibility with existing posts repository
  R when<R>({
    required R Function(T data) success,
    required R Function(String message, int? code) failure,
  }) {
    return fold(
      (data) => success(data),
      (error) {
        if (error is String) {
          return failure(error, null);
        } else {
          return failure(error.toString(), null);
        }
      },
    );
  }
}

class Success<T> extends Result<T> {
  final T data;
  const Success(this.data);
  
  @override
  R fold<R>(
    R Function(T) onSuccess,
    R Function(dynamic) onFailure,
  ) => onSuccess(data);
}

class Failure<T> extends Result<T> {
  final dynamic error;
  const Failure(this.error);
  
  @override
  R fold<R>(
    R Function(T) onSuccess,
    R Function(dynamic) onFailure,
  ) => onFailure(error);
}