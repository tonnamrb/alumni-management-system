import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:alumni_app/presentation/feed/bindings/posts_binding.dart';
import 'package:alumni_app/domain/repositories/posts_repository.dart';
import 'package:alumni_app/app/env/env_config.dart';

/// Simple test page to verify Posts API integration
class PostsTestPage extends StatelessWidget {
  const PostsTestPage({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    // Initialize posts binding
    PostsBinding().dependencies();
    
    return Scaffold(
      appBar: AppBar(
        title: const Text('Posts API Test'),
        backgroundColor: Colors.blue,
        foregroundColor: Colors.white,
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Icon(
              Icons.article,
              size: 80,
              color: Colors.blue,
            ),
            const SizedBox(height: 20),
            const Text(
              'Posts Integration Test',
              style: TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 20),
            ElevatedButton(
              onPressed: () async {
                try {
                  final postsRepo = Get.find<PostsRepository>();
                  final result = await postsRepo.getPosts(page: 1, pageSize: 5);
                  
                  result.when(
                    success: (posts) {
                      Get.snackbar(
                        'Success', 
                        'Found ${posts.length} posts',
                        backgroundColor: Colors.green.withOpacity(0.8),
                        colorText: Colors.white,
                      );
                    },
                    failure: (message, code) {
                      Get.snackbar(
                        'Success (API Connected)', 
                        'API connected but no posts: $message',
                        backgroundColor: Colors.orange.withOpacity(0.8),
                        colorText: Colors.white,
                      );
                    },
                  );
                } catch (e) {
                  Get.snackbar(
                    'Error', 
                    'Failed to connect: $e',
                    backgroundColor: Colors.red.withOpacity(0.8),
                    colorText: Colors.white,
                  );
                }
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.blue,
                foregroundColor: Colors.white,
                padding: const EdgeInsets.symmetric(horizontal: 30, vertical: 15),
              ),
              child: const Text(
                'Test Posts API',
                style: TextStyle(fontSize: 16),
              ),
            ),
            const SizedBox(height: 20),
            Text(
              'Backend should be running on\n${EnvConfigImpl.instance.apiBaseUrl}',
              textAlign: TextAlign.center,
              style: const TextStyle(
                color: Colors.grey,
                fontSize: 14,
              ),
            ),
          ],
        ),
      ),
    );
  }
}