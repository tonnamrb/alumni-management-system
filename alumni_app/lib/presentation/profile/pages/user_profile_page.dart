import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:alumni_app/presentation/profile/controllers/user_profile_controller.dart';
import 'package:alumni_app/shared/widgets/double_tap_like_widget.dart';

class UserProfilePage extends GetView<UserProfileController> {
  const UserProfilePage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Obx(() => Text(controller.targetUser.value?.name ?? 'Profile')),
        actions: [
          IconButton(
            icon: const Icon(Icons.more_vert),
            onPressed: () {
              // TODO: Show user options menu
            },
          ),
        ],
      ),
      body: Obx(() {
        if (controller.isLoading.value) {
          return const Center(
            child: CircularProgressIndicator(),
          );
        }
        
        final user = controller.targetUser.value;
        if (user == null) {
          return Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(
                  Icons.person_off,
                  size: 64,
                  color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.3),
                ),
                const SizedBox(height: 16),
                Text(
                  'User not found',
                  style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                    color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.6),
                  ),
                ),
              ],
            ),
          );
        }
        
        return SingleChildScrollView(
          child: Column(
            children: [
              // Profile Header
              Container(
                padding: const EdgeInsets.all(20),
                child: Column(
                  children: [
                    // Avatar
                    CircleAvatar(
                      radius: 50,
                      backgroundColor: Theme.of(context).colorScheme.primary,
                      child: user.avatar.isNotEmpty
                          ? ClipOval(
                              child: Image.network(
                                user.avatar,
                                width: 100,
                                height: 100,
                                fit: BoxFit.cover,
                                errorBuilder: (context, error, stackTrace) {
                                  return Text(
                                    user.name.isNotEmpty ? user.name[0].toUpperCase() : '?',
                                    style: const TextStyle(
                                      fontSize: 36,
                                      fontWeight: FontWeight.bold,
                                      color: Colors.white,
                                    ),
                                  );
                                },
                              ),
                            )
                          : Text(
                              user.name.isNotEmpty ? user.name[0].toUpperCase() : '?',
                              style: const TextStyle(
                                fontSize: 36,
                                fontWeight: FontWeight.bold,
                                color: Colors.white,
                              ),
                            ),
                    ),
                    
                    const SizedBox(height: 16),
                    
                    // User Name
                    Text(
                      user.name,
                      style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                      textAlign: TextAlign.center,
                    ),
                    
                    const SizedBox(height: 8),
                    
                    // Bio
                    Text(
                      user.bio,
                      style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                        color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.7),
                      ),
                      textAlign: TextAlign.center,
                      maxLines: 3,
                      overflow: TextOverflow.ellipsis,
                    ),
                    
                    const SizedBox(height: 16),
                    
                    // Stats Row
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                      children: [
                        _buildStatColumn(
                          context,
                          '${controller.userPosts.length}',
                          'Posts',
                        ),
                        _buildStatColumn(
                          context,
                          '${user.followersCount}',
                          'Followers',
                        ),
                        _buildStatColumn(
                          context,
                          '${user.followingCount}',
                          'Following',
                        ),
                      ],
                    ),
                    
                    const SizedBox(height: 20),
                    
                    // Action Buttons
                    SizedBox(
                      width: double.infinity,
                      child: Obx(() => ElevatedButton(
                        onPressed: controller.toggleFollow,
                        style: ElevatedButton.styleFrom(
                          backgroundColor: controller.isFollowing.value
                              ? Theme.of(context).colorScheme.outline
                              : Theme.of(context).colorScheme.primary,
                          foregroundColor: controller.isFollowing.value
                              ? Theme.of(context).colorScheme.onSurface
                              : Colors.white,
                        ),
                        child: Text(
                          controller.isFollowing.value ? 'Following' : 'Follow',
                        ),
                      )),
                    ),
                  ],
                ),
              ),
              
              // Divider
              const Divider(height: 1),
              
              // Posts Header
              Padding(
                padding: const EdgeInsets.all(16),
                child: Row(
                  children: [
                    Icon(
                      Icons.grid_on,
                      color: Theme.of(context).colorScheme.primary,
                    ),
                    const SizedBox(width: 8),
                    Text(
                      '${user.name}\'s Posts',
                      style: Theme.of(context).textTheme.titleMedium?.copyWith(
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ],
                ),
              ),
              
              // Posts Grid
              Obx(() {
                if (controller.userPosts.isEmpty) {
                  return Container(
                    padding: const EdgeInsets.all(40),
                    child: Column(
                      children: [
                        Icon(
                          Icons.grid_on,
                          size: 64,
                          color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.3),
                        ),
                        const SizedBox(height: 16),
                        Text(
                          'No posts yet',
                          style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                            color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.6),
                          ),
                        ),
                      ],
                    ),
                  );
                }
                
                return GridView.builder(
                  shrinkWrap: true,
                  physics: const NeverScrollableScrollPhysics(),
                  gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                    crossAxisCount: 3,
                    crossAxisSpacing: 1,
                    mainAxisSpacing: 1,
                  ),
                  itemCount: controller.userPosts.length,
                  itemBuilder: (context, index) {
                    final post = controller.userPosts[index];
                    return GestureDetector(
                      onTap: () => controller.viewPost(post),
                      child: Container(
                        decoration: BoxDecoration(
                          gradient: LinearGradient(
                            begin: Alignment.topLeft,
                            end: Alignment.bottomRight,
                            colors: [
                              Theme.of(context).colorScheme.primary.withValues(alpha: 0.7),
                              Theme.of(context).colorScheme.secondary.withValues(alpha: 0.7),
                            ],
                          ),
                        ),
                        child: Center(
                          child: Icon(
                            Icons.photo_library,
                            color: Colors.white.withValues(alpha: 0.8),
                            size: 32,
                          ),
                        ),
                      ),
                    );
                  },
                );
              }),
            ],
          ),
        );
      }),
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: 1, // Profile tab is at index 1
        onTap: (index) {
          if (index == 0) {
            // Navigate back to main navigation (Feed)
            Get.offAllNamed('/main');
          } else if (index == 1) {
            // Stay on profile - go back to main profile page
            Get.back();
          }
        },
        items: const [
          BottomNavigationBarItem(
            icon: Icon(Icons.home),
            label: 'Feed',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.person),
            label: 'Profile',
          ),
        ],
      ),
    );
  }
  
  Widget _buildStatColumn(BuildContext context, String count, String label) {
    return Column(
      children: [
        Text(
          count,
          style: Theme.of(context).textTheme.headlineSmall?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 4),
        Text(
          label,
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
            color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.7),
          ),
        ),
      ],
    );
  }
}

// Post Detail Page สำหรับ User Profile 
class UserProfilePostDetail extends StatelessWidget {
  final dynamic initialPost;
  final dynamic controller;
  final bool isVideoMode; // true = Instagram-style, false = normal feed
  
  const UserProfilePostDetail({
    super.key,
    required this.initialPost,
    required this.controller,
    this.isVideoMode = false,
  });

  @override
  Widget build(BuildContext context) {
    if (isVideoMode) {
      return _buildVideoModeView(context);
    } else {
      return _buildNormalFeedView(context);
    }
  }
  
  // Instagram-style vertical feed (สำหรับวิดีโอ)
  Widget _buildVideoModeView(BuildContext context) {
    final initialIndex = controller.userPosts.indexWhere((post) => post.id == initialPost.id);
    
    return Scaffold(
      backgroundColor: Colors.black,
      appBar: AppBar(
        title: Text(
          'Posts',
          style: TextStyle(color: Colors.white),
        ),
        backgroundColor: Colors.black,
        elevation: 0,
        iconTheme: const IconThemeData(color: Colors.white),
      ),
      body: Obx(() => PageView.builder(
        controller: PageController(initialPage: initialIndex >= 0 ? initialIndex : 0),
        scrollDirection: Axis.vertical,
        itemCount: controller.userPosts.length,
        itemBuilder: (context, index) {
          final post = controller.userPosts[index];
          return _buildVideoPostItem(context, post);
        },
      )),
      bottomNavigationBar: BottomNavigationBar(
        backgroundColor: Colors.black,
        selectedItemColor: Colors.white,
        unselectedItemColor: Colors.grey,
        currentIndex: 1, // Profile tab is at index 1 (since we're in user profile context)
        onTap: (index) {
          if (index == 0) {
            // Navigate back to main navigation (Feed)
            Get.offAllNamed('/main');
          } else if (index == 1) {
            // Stay on profile - go back to user profile page
            Get.back();
          }
        },
        items: const [
          BottomNavigationBarItem(
            icon: Icon(Icons.home),
            label: 'Feed',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.person),
            label: 'Profile',
          ),
        ],
      ),
    );
  }
  
  // Normal feed view (เหมือน feed หลัก)
  Widget _buildNormalFeedView(BuildContext context) {
    final initialIndex = controller.userPosts.indexWhere((post) => post.id == initialPost.id);
    
    return Scaffold(
      appBar: AppBar(
        title: Text('${initialPost.author}\'s Posts'),
      ),
      body: Obx(() => ListView.builder(
        controller: ScrollController(
          initialScrollOffset: initialIndex * 600.0, // ประมาณขนาดโพสต์
        ),
        itemCount: controller.userPosts.length,
        itemBuilder: (context, index) {
          final post = controller.userPosts[index];
          return _buildNormalPostCard(context, post);
        },
      )),
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: 1, // Profile tab is at index 1 (since we're in user profile context)
        onTap: (index) {
          if (index == 0) {
            // Navigate back to main navigation (Feed)
            Get.offAllNamed('/main');
          } else if (index == 1) {
            // Stay on profile - go back to user profile page
            Get.back();
          }
        },
        items: const [
          BottomNavigationBarItem(
            icon: Icon(Icons.home),
            label: 'Feed',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.person),
            label: 'Profile',
          ),
        ],
      ),
    );
  }

  // Normal post card style (เหมือน feed หลัก)
  Widget _buildNormalPostCard(BuildContext context, dynamic post) {
    return Card(
      margin: const EdgeInsets.symmetric(vertical: 4),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Post Header
          ListTile(
            leading: CircleAvatar(
              backgroundColor: Theme.of(context).colorScheme.primary,
              child: Text(
                post.author[0],
                style: const TextStyle(
                  color: Colors.white,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
            title: Text(
              post.author,
              style: const TextStyle(fontWeight: FontWeight.w600),
            ),
            subtitle: Text(_getTimeAgo(post.createdAt)),
          ),
          
          // Post Image with Double Tap Animation
          DoubleTapLikeWidget(
            post: post,
            onDoubleTap: () => controller.toggleLike(post.id),
            getCurrentLikeState: () {
              final currentPost = controller.userPosts.firstWhere((p) => p.id == post.id);
              return currentPost.isLiked;
            },
            child: Container(
              width: double.infinity,
              height: 300,
              child: Image.network(
                post.media,
                fit: BoxFit.cover,
                errorBuilder: (context, error, stackTrace) {
                  return Container(
                    color: Colors.grey[300],
                    child: const Icon(Icons.image, size: 50),
                  );
                },
              ),
            ),
          ),
          
          // Post Actions
          Padding(
            padding: const EdgeInsets.all(8),
            child: Row(
              children: [
                // Like Button
                IconButton(
                  icon: Icon(
                    post.isLiked ? Icons.favorite : Icons.favorite_border,
                    color: post.isLiked ? Colors.red : null,
                  ),
                  onPressed: () => controller.togglePostLike(post.id),
                ),
                Text('${post.likes}'),
                const SizedBox(width: 16),
                
                // Comment Button
                IconButton(
                  icon: const Icon(Icons.mode_comment_outlined),
                  onPressed: () => _showCommentsDialog(context, post, controller),
                ),
                Text('${post.comments.length}'),
              ],
            ),
          ),
          
          // Caption
          if (post.caption.isNotEmpty)
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
              child: RichText(
                text: TextSpan(
                  style: DefaultTextStyle.of(context).style,
                  children: [
                    TextSpan(
                      text: '${post.author} ',
                      style: const TextStyle(fontWeight: FontWeight.w600),
                    ),
                    TextSpan(text: post.caption),
                  ],
                ),
              ),
            ),
          
          // Comments preview
          if (post.comments.isNotEmpty)
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
              child: GestureDetector(
                onTap: () => _showCommentsDialog(context, post, controller),
                child: Text(
                  post.comments.length == 1 
                    ? 'View 1 comment'
                    : 'View all ${post.comments.length} comments',
                  style: TextStyle(
                    color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.6),
                  ),
                ),
              ),
            ),
            
          const SizedBox(height: 8),
        ],
      ),
    );
  }

  // Instagram-style video post (สำหรับ video mode)
  Widget _buildVideoPostItem(BuildContext context, dynamic post) {
    return Container(
      color: Colors.black,
      child: Column(
        children: [
          // User Header
          Container(
            color: Colors.black,
            padding: const EdgeInsets.all(16),
            child: Row(
              children: [
                CircleAvatar(
                  radius: 20,
                  backgroundColor: Theme.of(context).colorScheme.secondary,
                  child: Text(
                    post.author[0].toUpperCase(),
                    style: const TextStyle(
                      color: Colors.white,
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        post.author,
                        style: const TextStyle(
                          fontWeight: FontWeight.w600,
                          fontSize: 16,
                          color: Colors.white,
                        ),
                      ),
                      Text(
                        _getTimeAgo(post.createdAt),
                        style: TextStyle(
                          color: Colors.white.withValues(alpha: 0.7),
                          fontSize: 12,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
          
          // Post Image - Takes up most of the space
          Expanded(
            child: Container(
              width: double.infinity,
              child: Image.network(
                post.media,
                fit: BoxFit.contain,
                errorBuilder: (context, error, stackTrace) {
                  return Container(
                    color: Colors.grey[800],
                    child: const Icon(Icons.image, size: 50, color: Colors.white),
                  );
                },
              ),
            ),
          ),
          
          // Post Actions & Info
          Container(
            color: Colors.black,
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Actions Row
                Row(
                  children: [
                    // Like Button
                    GestureDetector(
                      onTap: () => controller.togglePostLike(post.id),
                      child: Row(
                        children: [
                          Icon(
                            post.isLiked ? Icons.favorite : Icons.favorite_border,
                            color: post.isLiked ? Colors.red : Colors.white,
                            size: 28,
                          ),
                          const SizedBox(width: 8),
                          Text(
                            '${post.likes}',
                            style: const TextStyle(color: Colors.white, fontWeight: FontWeight.w600),
                          ),
                        ],
                      ),
                    ),
                    
                    const SizedBox(width: 24),
                    
                    // Comment Button
                    GestureDetector(
                      onTap: () => _showCommentsDialog(context, post, controller),
                      child: Row(
                        children: [
                          const Icon(
                            Icons.mode_comment_outlined,
                            size: 26,
                            color: Colors.white,
                          ),
                          const SizedBox(width: 8),
                          Text(
                            '${post.comments.length}',
                            style: const TextStyle(color: Colors.white, fontWeight: FontWeight.w600),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
                
                const SizedBox(height: 12),
                
                // Post Caption
                if (post.caption.isNotEmpty) ...[
                  RichText(
                    text: TextSpan(
                      style: const TextStyle(color: Colors.white, fontSize: 14),
                      children: [
                        TextSpan(
                          text: '${post.author} ',
                          style: const TextStyle(fontWeight: FontWeight.w600),
                        ),
                        TextSpan(text: post.caption),
                      ],
                    ),
                  ),
                  const SizedBox(height: 8),
                ],
                
                // Comments preview
                if (post.comments.isNotEmpty) ...[
                  GestureDetector(
                    onTap: () => _showCommentsDialog(context, post, controller),
                    child: Text(
                      post.comments.length == 1 
                        ? 'View 1 comment'
                        : 'View all ${post.comments.length} comments',
                      style: TextStyle(
                        color: Colors.white.withValues(alpha: 0.7),
                        fontSize: 13,
                      ),
                    ),
                  ),
                ],
              ],
            ),
          ),
        ],
      ),
    );
  }
  
  String _getTimeAgo(DateTime dateTime) {
    final now = DateTime.now();
    final difference = now.difference(dateTime);
    
    if (difference.inDays > 0) {
      return '${difference.inDays}d';
    } else if (difference.inHours > 0) {
      return '${difference.inHours}h';
    } else if (difference.inMinutes > 0) {
      return '${difference.inMinutes}m';
    } else {
      return 'now';
    }
  }
  
  void _showCommentsDialog(BuildContext context, dynamic post, dynamic controller) {
    final commentController = TextEditingController();
    String? replyToUser;
    int? replyToCommentId;
    
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      builder: (context) => StatefulBuilder(
        builder: (context, setState) => Padding(
          padding: EdgeInsets.only(
            bottom: MediaQuery.of(context).viewInsets.bottom,
          ),
          child: Container(
            constraints: BoxConstraints(
              maxHeight: MediaQuery.of(context).size.height * 0.7,
            ),
            child: Column(
              children: [
                // Header
                Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(
                    border: Border(
                      bottom: BorderSide(
                        color: Theme.of(context).dividerColor,
                      ),
                    ),
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        'Comments',
                        style: Theme.of(context).textTheme.titleMedium,
                      ),
                      TextButton(
                        onPressed: () => Navigator.pop(context),
                        child: const Text('Done'),
                      ),
                    ],
                  ),
                ),
                
                // Comments List
                Expanded(
                  child: post.comments.isEmpty
                      ? Center(
                          child: Column(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              Icon(
                                Icons.comment_outlined,
                                size: 48,
                                color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.3),
                              ),
                              const SizedBox(height: 8),
                              Text(
                                'No comments yet',
                                style: TextStyle(
                                  color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.6),
                                ),
                              ),
                            ],
                          ),
                        )
                      : ListView.builder(
                          itemCount: post.comments.length,
                          itemBuilder: (context, index) {
                            final comment = post.comments[index];
                            return _UserProfileCommentTile(
                              post: post,
                              comment: comment,
                              controller: controller,
                              onReply: (String username, int commentId) {
                                setState(() {
                                  replyToUser = username;
                                  replyToCommentId = commentId;
                                  commentController.text = '@$username ';
                                });
                              },
                            );
                          },
                        ),
                ),
                
                // Reply indicator + Add Comment
                Column(
                  children: [
                    if (replyToUser != null)
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                        color: Theme.of(context).colorScheme.primaryContainer.withValues(alpha: 0.3),
                        child: Row(
                          children: [
                            Icon(
                              Icons.reply,
                              size: 16,
                              color: Theme.of(context).colorScheme.primary,
                            ),
                            const SizedBox(width: 8),
                            Text(
                              'Replying to $replyToUser',
                              style: TextStyle(
                                fontSize: 12,
                                color: Theme.of(context).colorScheme.primary,
                                fontWeight: FontWeight.w500,
                              ),
                            ),
                            const Spacer(),
                            GestureDetector(
                              onTap: () {
                                setState(() {
                                  replyToUser = null;
                                  replyToCommentId = null;
                                  commentController.clear();
                                });
                              },
                              child: Icon(
                                Icons.close,
                                size: 16,
                                color: Theme.of(context).colorScheme.primary,
                              ),
                            ),
                          ],
                        ),
                      ),
                    
                    Container(
                      padding: const EdgeInsets.all(16),
                      decoration: BoxDecoration(
                        border: Border(
                          top: BorderSide(
                            color: Theme.of(context).dividerColor,
                          ),
                        ),
                      ),
                      child: Row(
                        children: [
                          Expanded(
                            child: TextField(
                              controller: commentController,
                              decoration: InputDecoration(
                                hintText: replyToUser != null 
                                  ? 'Write a reply...' 
                                  : 'Add a comment...',
                                border: InputBorder.none,
                              ),
                            ),
                          ),
                          TextButton(
                            onPressed: () {
                              if (commentController.text.trim().isNotEmpty) {
                                if (replyToUser != null && replyToCommentId != null) {
                                  controller.addCommentReply(
                                    post.id, 
                                    replyToCommentId!, 
                                    commentController.text.trim(),
                                  );
                                } else {
                                  controller.addComment(post.id, commentController.text.trim());
                                }
                                
                                setState(() {
                                  replyToUser = null;
                                  replyToCommentId = null;
                                });
                                commentController.clear();
                              }
                            },
                            child: Text(replyToUser != null ? 'Reply' : 'Post'),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

// Comment Tile Widget สำหรับ User Profile
class _UserProfileCommentTile extends StatefulWidget {
  final dynamic post;
  final dynamic comment;
  final dynamic controller;
  final bool isReply;
  final Function(String username, int commentId)? onReply;
  
  const _UserProfileCommentTile({
    required this.post,
    required this.comment,
    required this.controller,
    this.isReply = false,
    this.onReply,
  });

  @override
  State<_UserProfileCommentTile> createState() => _UserProfileCommentTileState();
}

class _UserProfileCommentTileState extends State<_UserProfileCommentTile> {
  @override
  Widget build(BuildContext context) {
    return Container(
      padding: EdgeInsets.only(
        left: widget.isReply ? 32 : 16,
        right: 16,
        top: 8,
        bottom: 8,
      ),
      decoration: widget.isReply ? BoxDecoration(
        border: Border(
          left: BorderSide(
            color: Theme.of(context).colorScheme.primary.withValues(alpha: 0.3),
            width: 2,
          ),
        ),
      ) : null,
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Avatar
          CircleAvatar(
            radius: widget.isReply ? 12 : 16,
            backgroundColor: Theme.of(context).colorScheme.secondary,
            child: Text(
              widget.comment.user[0].toUpperCase(),
              style: TextStyle(
                color: Colors.white,
                fontSize: widget.isReply ? 10 : 12,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          
          const SizedBox(width: 12),
          
          // Content
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Username and comment text
                RichText(
                  text: TextSpan(
                    style: DefaultTextStyle.of(context).style,
                    children: [
                      TextSpan(
                        text: '${widget.comment.user} ',
                        style: const TextStyle(fontWeight: FontWeight.w600),
                      ),
                      TextSpan(text: widget.comment.text),
                    ],
                  ),
                ),
                
                const SizedBox(height: 4),
                
                // Actions row
                Row(
                  children: [
                    // Time ago
                    Text(
                      _getTimeAgo(widget.comment.createdAt),
                      style: TextStyle(
                        fontSize: 12,
                        color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.6),
                      ),
                    ),
                    
                    const SizedBox(width: 16),
                    
                    // Like button
                    InkWell(
                      onTap: () => _toggleCommentLike(),
                      borderRadius: BorderRadius.circular(20),
                      child: Padding(
                        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                        child: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Icon(
                              widget.comment.isLiked ? Icons.favorite : Icons.favorite_border,
                              size: 16,
                              color: widget.comment.isLiked ? Colors.red : Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.6),
                            ),
                            const SizedBox(width: 6),
                            Text(
                              '${widget.comment.likes}',
                              style: TextStyle(
                                fontSize: 13,
                                color: widget.comment.isLiked ? Colors.red : Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.6),
                                fontWeight: widget.comment.isLiked ? FontWeight.w600 : FontWeight.normal,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                    
                    const SizedBox(width: 16),
                    
                    // Reply button (only for main comments)
                    if (!widget.isReply && widget.onReply != null)
                      InkWell(
                        onTap: () => widget.onReply!(widget.comment.user, widget.comment.id),
                        borderRadius: BorderRadius.circular(20),
                        child: Padding(
                          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                          child: Text(
                            'Reply',
                            style: TextStyle(
                              fontSize: 13,
                              color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.7),
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                        ),
                      ),
                  ],
                ),
                
                // Replies
                if (!widget.isReply && widget.comment.replies.isNotEmpty) ...[
                  const SizedBox(height: 8),
                  ...widget.comment.replies.map((reply) => _UserProfileCommentTile(
                    post: widget.post,
                    comment: reply,
                    controller: widget.controller,
                    isReply: true,
                    onReply: null,
                  )),
                ],
              ],
            ),
          ),
        ],
      ),
    );
  }

  void _toggleCommentLike() {
    if (widget.isReply) {
      // Find parent comment
      final parentComment = widget.post.comments.firstWhere(
        (c) => c.replies.any((r) => r.id == widget.comment.id),
      );
      widget.controller.toggleReplyLike(widget.post.id, parentComment.id, widget.comment.id);
    } else {
      widget.controller.toggleCommentLike(widget.post.id, widget.comment.id);
    }
    // Force UI rebuild
    setState(() {});
  }

  String _getTimeAgo(DateTime dateTime) {
    final now = DateTime.now();
    final difference = now.difference(dateTime);
    
    if (difference.inDays > 0) {
      return '${difference.inDays}d';
    } else if (difference.inHours > 0) {
      return '${difference.inHours}h';
    } else if (difference.inMinutes > 0) {
      return '${difference.inMinutes}m';
    } else {
      return 'now';
    }
  }
}