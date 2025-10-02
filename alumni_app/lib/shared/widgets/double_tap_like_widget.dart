import 'package:flutter/material.dart';

/// Reusable Double Tap Like Widget with Animation
class DoubleTapLikeWidget extends StatefulWidget {
  final Widget child;
  final VoidCallback onDoubleTap;
  final dynamic post; // PostModel with isLiked property
  final bool Function()? getCurrentLikeState; // Callback to get current like state

  const DoubleTapLikeWidget({
    super.key,
    required this.child,
    required this.onDoubleTap,
    required this.post,
    this.getCurrentLikeState,
  });

  @override
  State<DoubleTapLikeWidget> createState() => _DoubleTapLikeWidgetState();
}

class _DoubleTapLikeWidgetState extends State<DoubleTapLikeWidget>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _scaleAnimation;
  late Animation<double> _opacityAnimation;
  
  bool _showAnimation = false;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 500),
      vsync: this,
    );
    
    _scaleAnimation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.elasticOut,
    ));
    
    _opacityAnimation = Tween<double>(
      begin: 1.0,
      end: 0.0,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: const Interval(0.5, 1.0),
    ));
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  void _handleDoubleTap() {
    // เก็บสถานะก่อน toggle เพื่อตรวจสอบว่าเป็นการ like หรือ unlike
    final bool wasLiked = widget.getCurrentLikeState?.call() ?? widget.post.isLiked;
    
    widget.onDoubleTap(); // เรียก toggleLike()
    
    // แสดง animation เฉพาะเมื่อเป็นการ Like (จาก false -> true)
    if (!wasLiked) {
      setState(() {
        _showAnimation = true;
      });
      
      _animationController.forward().then((_) {
        _animationController.reset();
        setState(() {
          _showAnimation = false;
        });
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onDoubleTap: _handleDoubleTap,
      child: Stack(
        alignment: Alignment.center,
        children: [
          widget.child,
          
          // Animation เฉพาะเมื่อ Double Tap และ Like เท่านั้น
          if (_showAnimation)
            Positioned.fill(
              child: Center(
                child: AnimatedBuilder(
                  animation: _animationController,
                  builder: (context, child) {
                    return Transform.scale(
                      scale: _scaleAnimation.value,
                      child: Opacity(
                        opacity: _opacityAnimation.value,
                        child: const Icon(
                          Icons.favorite,
                          color: Colors.red,
                          size: 80,
                        ),
                      ),
                    );
                  },
                ),
              ),
            ),
        ],
      ),
    );
  }
}