# OrderService

## This service processes the new orders and mark the status as `processed` in database.

It is a hosted service that runs in the background. It keeps on waiting for the new order request and process it the moment a new order is placed. The order status is updated in mongodb as "processed" after order completion.
